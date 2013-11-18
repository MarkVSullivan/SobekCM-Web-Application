// Functions related to the google-map based searching

// Map variables
var sobekcm_map;
var bounds;
var selected_bounds;

function show_map_instructions() {
	var instructionsDiv = document.getElementById("MapInstructions");
	instructionsDiv.style.display = 'block';

	var instructionsDiv = document.getElementById("MapInstructionsLink");
	instructionsDiv.style.display = 'none';

	return false;
}

function address_keydown(event, what) {
	var return_caught = false;
	if (document.all) {
		if (event.keyCode == 13) {
			return_caught = true;
		}
	}

	else if (document.getElementById) {
		if (event.which == 13) {
			return_caught = true;
		}
	}

	else if (document.layers) {
		if (event.which == 13) {
			return_caught = true;
		}
	}

	if (return_caught) {
		map_address_geocode();
		return false;
	}
	else {
		address_box_changed(what);
	}
}

function address_box_changed(what) {
	if ((what.value == '') || ( what.value == what.defaultValue ))
	{
		// Disable the find address button
		var searchButton = document.getElementById("findButton");
		if (searchButton != null) {
			searchButton.disabled = true;
		}
	}
	else {
		// Enable the find address button
		var searchButton = document.getElementById("findButton");
		if (searchButton != null) {
			searchButton.disabled = false;
		}
	}
}

function show_coordinates()
{
	var coordinate_div = document.getElementById("map_coordinates_div");
	coordinate_div.style.display = '';
	
	var coordinate_text = document.getElementById("coordinate_text_span");
	coordinate_text.innerHTML = "Alternatively, you can search by latitude/longitude.";
	
	return false;
}

function load_map(latitude, longitude, zoom, mapdiv) 
{
	// Create the google map
	var map_center = new google.maps.LatLng(latitude, longitude);
	var map_options = {
		zoom: zoom,
		center: map_center,
		mapTypeId: google.maps.MapTypeId.HYBRID,
		streetViewControl: false
	};
	sobekcm_map = new SobekCM.Map(mapdiv, map_options);

	// Declare a new lat lng bounds to hold all the points (bounds) added
	bounds = new google.maps.LatLngBounds();
	selected_bounds = new google.maps.LatLngBounds();
}


// Create the map for the main google map search page
function load_search_map(latitude, longitude, zoom, mapdiv ) {

	// Create the google map
	var map_center = new google.maps.LatLng(latitude, longitude);
	var map_options = {
		zoom: zoom,
		center: map_center,
		mapTypeId: google.maps.MapTypeId.ROADMAP,
		streetViewControl: false
	};
	sobekcm_map = new SobekCM.Map(mapdiv, map_options);
	
	// Add an interest in the maps new selection "event"
	sobekcm_map.OnNewSelection = function(points) {
		var textbox1 = document.getElementById("Textbox1");
		var textbox2 = document.getElementById("Textbox2");
		var textbox3 = document.getElementById("Textbox3");
		var textbox4 = document.getElementById("Textbox4");
	
		if (points.length == 2) {
			textbox1.value = points[0];
			textbox2.value = points[1];
			textbox3.value = '';
			textbox4.value = '';
		}

		if (points.length == 4) {
			textbox1.value = points[0];
			textbox2.value = points[1];
			textbox3.value = points[2];
			textbox4.value = points[3];
		}

		// Enable the search button 
		var searchButton = document.getElementById("searchButton");
		searchButton.disabled = false;
	};

	// Add an interest in the maps selection begins "event"
	sobekcm_map.OnNewSelectionStarted = function() {

		// Clear all the points on the form
		var textbox1 = document.getElementById("Textbox1");
		var textbox2 = document.getElementById("Textbox2");
		var textbox3 = document.getElementById("Textbox3");
		var textbox4 = document.getElementById("Textbox4");
		textbox1.value = '';
		textbox2.value = '';
		textbox3.value = '';
		textbox4.value = '';

		// Return the address to the initial value
		var addressBox = document.getElementById("AddressTextBox");
		if (addressBox != null) {
			addressBox.value = '';
		}

		// Disable the search button 
		var searchButton = document.getElementById("searchButton");
		searchButton.disabled = true;

		// Disable the find address button
		var searchButton = document.getElementById("findButton");
		if (searchButton != null) {
			searchButton.disabled = true;
		}
	};

	// Add the map tool type
	sobekcm_map.setMapTool(SobekCM_MapTool_Type_Enum.standard);

	// Disable the search button in javascript
	var searchButton = document.getElementById("searchButton");
	searchButton.disabled = true;

	// Disable the find address button
	var searchButton = document.getElementById("findButton");
	if (searchButton != null) {
		searchButton.disabled = true;
	}

	// Declare a new lat lng bounds to hold all the points (bounds) added
	bounds = new google.maps.LatLngBounds();
}

// Disable the point searching on the map
function disable_point_searching() {
	sobekcm_map.disable_point_search();
}

// Add the map key to the map
function add_key(search_type, areas_shown, points_shown, area_name) {

	sobekcm_map.addMapKey(search_type, areas_shown, points_shown, area_name);
}

// Add a point selection to this map
function add_selection_point( lat, lng, zoom )
{
	var point = new google.maps.LatLng( lat, lng );
	sobekcm_map.set_selection_point(point, true, true, zoom);
}

// Add a rectangular selection to this map
function add_selection_rectangle(lat1, lng1, lat2, lng2) {
	var point1 = new google.maps.LatLng(lat1, lng1);
	var point2 = new google.maps.LatLng(lat2, lng2);
	sobekcm_map.set_selection_rectangle(point1, point2, true, false);

	bounds.extend(point1);
	bounds.extend(point2);
}

// Zoom to the bounds
function zoom_to_bounds() {
	sobekcm_map.zoom_to_bounds(bounds);
	return false;
}

// Zoom to the bounds of all the selected points and polygons
function zoom_to_selected() {
	sobekcm_map.zoom_to_bounds(selected_bounds);
	return false;
}

// Add a standard marker to this map
function add_point(lat, lng, label) 
{
	var point = new google.maps.LatLng(lat, lng);
	bounds.extend(point);
	sobekcm_map.add_point(point, label);
}

// Add a standard polygon to this map
function add_polygon( points, selected, label, link ) {
	// Determine bounds
	var point = 0;
	while ( point < points.length )
	{
		bounds.extend(points[point]);
		if (selected) {
			selected_bounds.extend(points[point]);
		}
		point++;
	}

	if (selected )
		sobekcm_map.add_polygon(points, '#22bb22', '#33cc00', 4, label, link);
	else
		sobekcm_map.add_polygon(points, '#3333FF', '#3333FF', 0, label, link);
}

function add_rectangle(lat1, long1, lat2, long2) {

	sobekcm_map.add_rectangle(lat1, long1, lat2, long2, "#cc3333");
}

// Send the user to the UFDC search results page for their basic search
function map_search_sobekcm( root )
{
	// Collect and trim the users's search string
	var term = trimString(document.search_form.Textbox1.value) + "," + trimString( document.search_form.Textbox2.value ) + "," +  trimString(document.search_form.Textbox3.value) + "," + trimString( document.search_form.Textbox4.value );

	if (term.length > 0) {

		// Build the destination url by placing the selection option first
		var url = root + "?coord=" + term;
		if (root.indexOf("?") > 0)
			url = root + "&coord=" + term;
		
		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Send the user to the UFDC search results page for their basic search
function map_item_search_sobekcm(root) {
	// Collect and trim the users's search string
	var term = trimString(document.itemNavForm.Textbox1.value) + "," + trimString(document.itemNavForm.Textbox2.value) + "," + trimString(document.itemNavForm.Textbox3.value) + "," + trimString(document.itemNavForm.Textbox4.value);

	if (term.length > 0) {
		// Build the destination url by placing the selection option first
		var url = root + "&coord=" + term;

		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Get the address and put a dot on the map
function map_address_geocode( )
{
	var addressBox = document.getElementById("AddressTextBox");
	var address = addressBox.value;

	var geocoder = new google.maps.Geocoder();
	
	if (geocoder) {
		geocoder.geocode({ 'address': address }, function(results, status) {
		if (status == google.maps.GeocoderStatus.OK) 
			{
				sobekcm_map.set_selection_point(results[0].geometry.location, true, true, 15);
			} else {
				alert("Geocode was not successful for the following reason: " + status);
			}
		});
	}
}

// Takes the coordinates entered into the coordinate boxes, makes this the new selection,
// and subsequently zooms to that selection
function locate_by_coordinates() {

	var lat1 = parseFloat(document.search_form.Textbox1.value);
	var lng1 = parseFloat(document.search_form.Textbox2.value);
	var lat2 = parseFloat(document.search_form.Textbox3.value);
	var lng2 = parseFloat(document.search_form.Textbox4.value);

	// To do anything, the first point must be valid
	if (( isNaN(lat1)) || ( isNaN(lng1)))
		return;

	// Was this a box selection?
	if (( !isNaN(lat2)) && ( !isNaN(lng2))) {
		// Two points indicatd, so set the selection rectangle
		add_selection_rectangle(lat1, lng1, lat2, lng2);

		// Zoom to the new bounds
		zoom_to_bounds();
	}
else {
		// Ensure the values in the text box are really empty now
		document.search_form.Textbox3.value = '';
		document.search_form.Textbox4.value = '';

		// Set the single select point
		add_selection_point(lat1, lng1, 15);
	 }
}



// Trim the input string from the search box
function trimString (str) 
{
  str = this != window? this : str;
  return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}
