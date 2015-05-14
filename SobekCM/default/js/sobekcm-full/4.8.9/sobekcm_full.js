
// Initialize the search term and the button to remove the search term
function init_search_term(division, button) {
	$("#" + division).mouseenter(function () {
		$('#' + division).removeClass('sbkPrsw_SearchTerm').addClass('sbkPrsw_SearchTermHovered');
		$('#' + button).removeClass('sbkPrsw_RemoveSearchTerm').addClass('sbkPrsw_RemoveSearchTermHovered');
	});
	$("#" + division).mouseleave(function () {
		$('#' + division).removeClass('sbkPrsw_SearchTermHovered').addClass('sbkPrsw_SearchTerm');
		$('#' + button).removeClass('sbkPrsw_RemoveSearchTermHovered').addClass('sbkPrsw_RemoveSearchTerm');
	});
}


// Function to add something to the onload event 
function addLoadEvent(func) {
	if (func != undefined) {
		var oldonload = window.onload;
		if (typeof window.onload != 'function') {
			window.onload = func;
		} else {
			window.onload = function () {
				if (oldonload) {
					oldonload();
				}
				func();
			};
		}
	}
}


// Standard functions used throughout the application
function set_subaggr_display(new_value) {
	var hidden_value = document.getElementById("show_subaggrs");
	if (hidden_value != null) {
		hidden_value.value = new_value;
		document.search_form.submit();
	}

	return false;
}

//toggles the specific filter id
function toggleFilterTitle(id) {
    if ((document.getElementById(id).style.display == "none") || (document.getElementById(id).style.display == "")) {
        document.getElementById(id).style.display = "block";
    } else {
        document.getElementById(id).style.display = "none";
    }
}


function set_facet(facet, new_value) {
	var hidden_value = document.getElementById("facet");
	var previous_value = hidden_value.value;
	previous_value = setCharAt(previous_value, facet, new_value);
	hidden_value.value = previous_value;

	document.itemNavForm.submit();

	return false;
}

function setCharAt(str, index, chr) {
	if (index > str.length - 1) return str;
	return str.substr(0, index) + chr + str.substr(index + 1);
}

// Hide and show the internal header (for internal users only)
function hide_internal_header() {
	var hidden_value = document.getElementById("internal_header_action");
	hidden_value.value = "hide";
	document.internalHeaderForm.submit();
	return false;
}

function show_internal_header() {
	var hidden_value = document.getElementById("internal_header_action");
	hidden_value.value = "show";
	document.internalHeaderForm.submit();
	return false;
}

function save_internal_notes() {
	var hidden_value = document.getElementById("internal_header_action");
	hidden_value.value = "save_comments";
	document.internalHeaderForm.submit();
	return false;
}

function open_access_restrictions() {
	var el = document.getElementById('access_restrictions_div');
	if (el != null) {
		if (el.style.display == 'none') { el.style.display = 'block'; }
		else { el.style.display = 'none'; }
	}

	return false;
}

function set_item_access(new_value) {
	var hidden_value = document.getElementById("permissions_action");
	hidden_value.value = new_value;
	document.itemNavForm.submit();
	return false;
}

// Trim the input string from the search box
function trimString(str) {
	str = this != window ? this : str;
	return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}

// TEXT BOX ENTER AND LEAVE 
function textbox_enter(elementid, className) {
	document.getElementById(elementid).className = className;
}

function textbox_leave(elementid, className) {
	document.getElementById(elementid).className = className;
}

function selectbox_leave(elementid, leaveClassName, initClassName) {
	var selectElement = document.getElementById(elementid);
	if ((selectElement.value.indexOf('Select') == 0) || (selectElement.value.indexOf('(none)') == 0))
		selectElement.className = initClassName;
	else
		selectElement.className = leaveClassName;
}

function textbox_enter_default_value(what, className) {
	if (what.value == what.defaultValue) {
		what.value = '';
	}
	what.className = className + '_focused';
}

function textbox_leave_default_value(what, className) {
	if (what.value == '') {
		what.className = className + '_initial';
		what.value = what.defaultValue;
	}
	else {
		what.className = className;
	}
}

function Add_Circular_Progress() {
	var circular_div = document.getElementById("circular_progress");
	circular_div.className = "shown_progress";
}

function focus_element(focusname) {
	var focusfield = document.getElementById(focusname);
	if (focusfield != null) {
		focusfield.focus();
	}
}


function send_contact_email() {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'email';

	document.email_form.submit();
	return false;
}

function remove_aggr_from_myhome(aggregationCode) {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'remove_aggregation';
	var hiddenfield2 = document.getElementById('aggregation');
	hiddenfield2.value = aggregationCode;

	document.search_form.submit();

	return false;
}

function remove_aggregation() {
	var input_box = confirm("Do you really want to remove this from your personlized home page?");
	if (input_box == true) {
		// Populate the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'remove_aggregation';

		document.search_form.submit();
	}

	return false;
}

function add_aggregation() {
	var input_box = confirm("Do you really want to add this to your personlized home page?");
	if (input_box == true) {
		// Populate the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'add_aggregation';

		document.search_form.submit();
	}

	return false;
}

function make_folder_private(foldername) {
	var input_box = confirm("Do you really want to make this folder private?");
	if (input_box == true) {
		// Populate the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'private_folder';
		var hiddenfield2 = document.getElementById('aggregation');
		hiddenfield2.value = foldername;
		document.search_form.submit();
	}

	return false;
}


function delete_folder(folder) {
	var input_box = confirm("Do you really want to delete this folder?");
	if (input_box == true) {
		// Populate the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'delete_folder';
		var hiddenfield2 = document.getElementById('bookshelf_items');
		hiddenfield2.value = folder;

		document.itemNavForm.submit();
	}

	return false;
}

function refresh_bookshelves() {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'refresh_folder';

	document.itemNavForm.submit();

	return false;
}

function change_folder_visibility(folder, new_status) {
	var input_box = confirm("Do you really want to make this folder " + new_status + "?");
	if (input_box == true) {
		// Populate the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'folder_visibility';
		var hiddenfield2 = document.getElementById('bookshelf_items');
		hiddenfield2.value = folder;
		var hiddenfield3 = document.getElementById('bookshelf_params');
		hiddenfield3.value = new_status;

		document.itemNavForm.submit();
	}

	return false;
}

// Changes the checkbox values for every item in a bookshelf
function bookshelf_all_check_clicked(items_included) {
	// Get the number of children
	var item_count = eval(items_included);

	// Get this checkbox
	var mainCheckBox = document.getElementById('bookshelf_all_check');
	var mainCheckedValue = mainCheckBox.checked;

	for (i = 0 ; i < item_count ; i++) {
		var childCheckBox = document.getElementById('item_select_' + i.toString());
		childCheckBox.checked = mainCheckedValue;
	}
}

// Remove a number of items from a bookshelf
function remove_all(items_included) {
	// Get the number of children
	var item_count = eval(items_included);
	var selected_count = 0;
	var selected_ids = '';

	for (i = 0 ; i < item_count ; i++) {
		var childCheckBox = document.getElementById('item_select_' + i.toString());
		if (childCheckBox.checked == true) {
			selected_count++;
			if (selected_ids.length > 0)
				selected_ids = selected_ids + "|" + childCheckBox.value;
			else
				selected_ids = childCheckBox.value;
		}
	}

	var input_box = false;
	if (selected_count == 0) {
		alert('No items selected!\n\nChoose one or more items by using the checkboxes.');
	}
	else {

		if (selected_count == 1) {
			input_box = confirm("Do you really want to remove this item from this bookshelf?");
		}
		else {
			input_box = confirm("Do you really want to remove these " + selected_count + " items from this bookshelf?");
		}
	}

	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'remove';
		var hiddenfield2 = document.getElementById('bookshelf_items');
		hiddenfield2.value = selected_ids;

		document.itemNavForm.submit();

		return false;
	}

	// Return false to prevent return trip to the server
	return false;
}

// Move a number of items from a bookshelf
function move_all(items_included) {
	// Get the number of children
	var item_count = eval(items_included);
	var selected_count = 0;
	var selected_ids = '';

	for (i = 0 ; i < item_count ; i++) {
		var childCheckBox = document.getElementById('item_select_' + i.toString());
		if (childCheckBox.checked == true) {
			selected_count++;
			if (selected_ids.length > 0)
				selected_ids = selected_ids + "|" + childCheckBox.value;
			else
				selected_ids = childCheckBox.value;
		}
	}

	var input_box = true;
	if (selected_count == 0) {
		alert('No items selected!\n\nChoose one or more items by using the checkboxes.');
		return false;
	}
	else {
		if (selected_count > 1) {
			input_box = confirm("Do you really want to move these " + selected_count + " to another bookshelf?");
		}
	}

	if (input_box == true) {
		// Populate the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'move';
		var hiddenfield2 = document.getElementById('bookshelf_items');
		hiddenfield2.value = selected_ids;

		// Set the legend correctly
		var legend_div = document.getElementById('move_legend');
		if (selected_count == 1)
			legend_div.innerHTML = 'Select new bookshelf for this item';
		else
			legend_div.innerHTML = 'Select new bookshelf for these ' + selected_count + ' items';

		// Toggle the move form
		blanket_size('move_item_form', 'bookshelf_all_move', 400);
		window_pos('move_item_form', 532);
		toggle('blanket_outer');
		toggle('move_item_form');
	}


	// Return false to prevent return trip to the server
	return false;
}

// Remove a search
function delete_search(id) {
	var input_box = confirm("Do you really want to remove this search from your list of saved searches?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'remove';
		var hiddenfield2 = document.getElementById('folder_id');
		hiddenfield2.value = id;

		document.itemNavForm.submit();
	}

	// Return false to prevent another return trip to the server
	return false;
}


// Remove an item from a bookshelf
function remove_item(id) {
	var input_box = confirm("Do you really want to remove this item from this bookshelf?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'remove';
		var hiddenfield2 = document.getElementById('bookshelf_items');
		hiddenfield2.value = id;

		document.itemNavForm.submit();


	}

	// Return false to prevent another return trip to the server
	return false;
}

function delete_tag(id) {
	var input_box = confirm("Do you really want to remove this descriptive tag?");
	if (input_box == true) {
		// Populate the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'delete_tag_' + id;

		document.itemNavForm.submit();
	}

	return false;
}




function preferences(root) {
	var language_val = document.preferences_form.languageDropDown.value;
	var url = "http://ufdc.ufl.edu/" + root + language_val;
	if (document.preferences_form.lowRadioButton.Checked)
		url = url + '&ba=s';
	window.location.href = url;
}

// Send the user to the SobekCM search results page for a search within a single item
function item_search_sobekcm(root) {
	// Collect and trim the users's search string
	var term = trimString(document.itemNavForm.searchTextBox.value);

	if (term.length > 0) {
		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(",", " ").replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

		// Build the destination url by placing the selection option first
		var url = root + "?search=" + term;
		if (root.indexOf("?") > 0)
			url = root + "&search=" + term;

		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Listen to the entry into the single item search box
function item_search_keytrap(event, root) {

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
		event.returnValue = false;
		event.cancel = true;
		item_search_sobekcm(root);
		return false;
	}
}



function sort_results(root) {
	var sort_val = document.sort_form.sorter_input.value;
	var url = root + "?o=" + sort_val;
	if (root.indexOf("?") > 0)
		url = root + "&o=" + sort_val;
	window.location.href = url;
}

function sort_private_list(root) {
	var sort_val = document.search_form.sorter_input.value;
	var url = root + "?o=" + sort_val;
	if (root.indexOf("?") > 0)
		url = root + "&o=" + sort_val;
	window.location.href = url;
}

// Function used to pull item count between two arbitrary dates
function arbitrary_item_count(root) {
	var first_value = document.statistics_form.date1input.value.replace('mm/dd/yyyy', '').replace('/', '-').replace('/', '-');
	var second_value = document.statistics_form.date2input.value.replace('mm/dd/yyyy', '').replace('/', '-').replace('/', '-');

	if (first_value.length == 0) {
		return false;
	}
	else {
		if (second_value.length > 0) {
			window.location.href = root + '/' + first_value + '-' + second_value;
		}
		else {
			window.location.href = root + '/' + first_value;
		}
	}
}

// Function is used for pulling the statistical information for a new date
function date_jump_sobekcm(root) {
	// Collect the values from the drop boxes
	var date_result = document.statistics_form.date1_selector.value + document.statistics_form.date2_selector.value;
	var url = root + '/' + date_result;
	window.location.href = url;
}

// Function is used for pulling the statistical information for a new collection
function collection_jump_sobekcm(root) {
	// Collect the values from the drop boxes
	var collection_selected = document.statistics_form.collection_selector.value;
	var url = root + '/' + collection_selected;
	window.location.href = url;
}

// Function is used for pulling the statistical information for a new collection
function collection_jump_sobekcm2(root) {
	// Collect the values from the drop boxes
	var collection_selected = document.statistics_form.collection_selector2.value;
	var url = root + '/' + collection_selected;
	window.location.href = url;
}

// INTERNAL HEADER FUNCTIONS
function internalTrapKD(event, root) {

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
		event.returnValue = false;
		event.cancel = true;
		internal_search(root);
		return false;
	}
}

function internal_search(root) {

	// Collect and trim the users's search string
	var term = document.internalHeaderForm.internalSearchTextBox.value;
	term = (term.replace(/^\s+/g, '').replace(/\s+$/g, '')).replace(",", " ");
	if (term.length > 0) {

		// replace ' or ' and ' and ' and ' not ' in the query
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");;

		var fields = document.internalHeaderForm.internalDropDownList.value;

		// Build the destination url by placing the selection option first
		var url = root + "?t=" + term + "&f=" + fields;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term + "&f=" + fields;

		// Change the the browser location to the new url
		window.location.href = url;
	}
}

function show_trace_route() {
	var traceroutediv = document.getElementById("sbkHmw_TraceRouter");
	traceroutediv.style.display = 'block';

	var link = document.getElementById("sbkHmw_TraceRouterShowLink");
	link.style.visibility = 'hidden';
	return false;
}

function show_header_info() {
	var traceroutediv = document.getElementById("sbkSbia_HeaderInfoDiv");
	traceroutediv.style.display = 'inline';

	var link = document.getElementById("sbkSbia_HeaderInfoDivShowLink");
	link.style.visibility = 'hidden';
	return false;
}

// This contains all the basic search javascript which takes the user's entries ( client-side )
// and converts to the search URL and requests the appropriate results to match the search

function fnTrapKD(event, type, arg1, arg2, browseurl) {

	var return_caught = false;
	if (document.all) {
		if (event.keyCode == 13) {
			return_caught = true;
		}
	} else if (document.getElementById) {
		if (event.which == 13) {
			return_caught = true;
		} else if (document.layers) {
			if (event.which == 13) {
				return_caught = true;
			}
		}
	}

	if (return_caught) {
		event.returnValue = false;
		event.cancel = true;

		if (arg2.length == 0) {
			if (type == 'basic')
				basic_search_sobekcm(arg1, browseurl);
			if (type == 'basicyears')
				basic_search_years_sobekcm(arg1, browseurl);
			if (type == 'metadata')
				metadata_search_sobekcm(arg1, browseurl);
			if (type == 'newspaper')
				newspaper_search_sobekcm(arg1, browseurl);
			if (type == 'dloc')
				dloc_search_sobekcm(arg1, browseurl);
			if (type == 'text')
				fulltext_search_sobekcm(arg1, browseurl);
		} else {
			if (type == 'basic')
				basic_select_search_sobekcm(arg1, arg2);
			if (type == 'metadata')
				metadata_select_search_sobekcm(arg1, arg2);
			if (type == 'newspaper')
				newspaper_select_search_sobekcm(arg1, arg2);
			if (type == 'dloc')
				dloc_select_search_sobekcm(arg1, arg2);
			if (type == 'text')
				fulltext_select_search_sobekcm(arg1, arg2);
		}

		return false;
	}
}


// Advanced search
function advanced_search_sobekcm(root, browseurl ) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.Textbox1.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox2.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox3.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox4.value).replace(",", "+").replace(" ", "+");
	var fields = document.search_form.Dropdownlist1.value + "," + document.search_form.andOrNotBox1.value + document.search_form.Dropdownlist2.value + "," + document.search_form.andOrNotBox2.value + document.search_form.Dropdownlist3.value + "," + document.search_form.andOrNotBox3.value + document.search_form.Dropdownlist4.value;

    // Look for the MIMETYPE filter
	if (document.getElementById('sbkAsav_mimetypeCheck') != null)
	{
	    if (document.getElementById('sbkAsav_mimetypeCheck').checked) {
	        term = term + ',NONE';
	        fields = fields + ',-MI';
	    } 
	} 


	if (term.length > 3) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		//term = term.toLowerCase().replace(" or "," =").replace(" and ", " ").replace(" not "," -");

		// Determine if a precision was selected
		var contains_radio = document.getElementById('precisionContains');
		if ((contains_radio != null) && (contains_radio.checked == true))
			root = root.replace("results", contains_radio.value);

		var results_radio = document.getElementById('precisionResults');
		if ((results_radio != null) && (results_radio.checked == true))
			root = root.replace("results", results_radio.value);

		var like_radio = document.getElementById('precisionLike');
		if ((like_radio != null) && (like_radio.checked == true))
			root = root.replace("results", like_radio.value);

		// Build the destination url by placing the selection option first
		var url = root + "?t=" + term + "&f=" + fields;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term + "&f=" + fields;

		// Change the the browser location to the new url
		window.location.href = url;
	}
	else if (browseurl.length > 0) {
	    window.location.href = browseurl;
	}
}

// Advanced search in an aggregation with children selectable
function advanced_select_search_sobekcm(root, next_level) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.Textbox1.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox2.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox3.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox4.value).replace(",", "+").replace(" ", "+");
	var fields = document.search_form.Dropdownlist1.value + "," + document.search_form.andOrNotBox1.value + document.search_form.Dropdownlist2.value + "," + document.search_form.andOrNotBox2.value + document.search_form.Dropdownlist3.value + "," + document.search_form.andOrNotBox3.value + document.search_form.Dropdownlist4.value;

	if (term.length > 0) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

		// Build the destination url by placing the selection option first
		var url = root + "?t=" + term + "&f=" + fields;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term + "&f=" + fields;

		// Do additional work if there are checkboxes on this form
		if (document.search_form.checkgroup.length > 0) {
			// Check to see if any of the checkboxes are NOT checked
			var test = true;
			for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
				if (document.search_form.checkgroup[i].checked == false) {
					test = false;
				}
			}

			// If some were unchecked, include the codes for the checked boxes
			var found = 0;
			if (test == false) {
				if (next_level.indexOf("s") >= 0) {
					url = url + "&" + next_level + "-";

					for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
						if (document.search_form.checkgroup[i].checked == false) {
							if (found > 0) {
								url = url + "," + document.search_form.checkgroup[i].value;
							}
							else {
								url = url + document.search_form.checkgroup[i].value;
							}
							found++;
						}
					}
				}
				else {
					url = url + "&" + next_level + ".";

					for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
						if (document.search_form.checkgroup[i].checked == true) {
							url = url + document.search_form.checkgroup[i].value + ",";
						}
					}
				}
			}
		}

		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Advanced search
function advanced_search_years_sobekcm(root, browseurl) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.Textbox1.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox2.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox3.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox4.value).replace(",", "+").replace(" ", "+");
	var fields = document.search_form.Dropdownlist1.value + "," + document.search_form.andOrNotBox1.value + document.search_form.Dropdownlist2.value + "," + document.search_form.andOrNotBox2.value + document.search_form.Dropdownlist3.value + "," + document.search_form.andOrNotBox3.value + document.search_form.Dropdownlist4.value;

	// Get the year range data first
	var year1 = document.search_form.YearDropDown1.value;
	var year2 = document.search_form.YearDropDown2.value;
	var year_url_append = '';
	if ((year1.length > 0) && (year1 != 'ZZ')) {
		year_url_append = "&yr1=" + year1;

		if ((year2.length > 0) && (year2 != 'ZZ')) {
			year_url_append = year_url_append + "&yr2=" + year2;
		} else {
			year_url_append = year_url_append + "&yr2=" + new Date().getFullYear();
		}
	}

	if (term.length > 3) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		//term = term.toLowerCase().replace(" or "," =").replace(" and ", " ").replace(" not "," -");

		// Determine if a precision was selected
		var contains_radio = document.getElementById('precisionContains');
		if ((contains_radio != null) && (contains_radio.checked == true))
			root = root.replace("results", contains_radio.value);

		var results_radio = document.getElementById('precisionResults');
		if ((results_radio != null) && (results_radio.checked == true))
			root = root.replace("results", results_radio.value);

		var like_radio = document.getElementById('precisionLike');
		if ((like_radio != null) && (like_radio.checked == true))
			root = root.replace("results", like_radio.value);

		// Build the destination url by placing the selection option first
		var url = root + "?t=" + term + "&f=" + fields + year_url_append;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term + "&f=" + fields + year_url_append;

		// Change the the browser location to the new url
		window.location.href = url;
	}
	else if (browseurl.length > 0) {
	    window.location.href = browseurl;
	}
}

// Full text
function fulltext_search_sobekcm(root, browseurl) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.u_search.value).replace(",", " ");
	if ((term.length > 0) && (term != '*')) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");;

		// Build the destination url by placing the selection option first and redirect
		var url = root + "?text=" + term;
		if (root.indexOf("?") > 0)
			url = root + "&text=" + term;
		window.location.href = url;
	}
	else if (browseurl.length > 0) {
		window.location.href = browseurl;
	}
}

// Basic search in an aggregation with children selectable
function fulltext_select_search_sobekcm(root, next_level) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.u_search.value).replace(",", " ");
	if (term.length > 0) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");;

		// Build the destination url by placing the selection option first
		var url = root + "?text=" + term;
		if (root.indexOf("?") > 0)
			url = root + "&text=" + term;

		// Do additional work if there are checkboxes on this form
		if (document.search_form.checkgroup.length > 0) {
			// Check to see if any of the checkboxes are NOT checked
			var test = "true";
			for (i = 0; i < document.search_form.checkgroup.length; i++) {
				if (document.search_form.checkgroup[i].checked == false) {
					test = "false";
				}
			}

			// If some were unchecked, include the codes for the checked boxes
			if (test == "false") {
				url = url + "&" + next_level + ".";

				for (i = 0; i < document.search_form.checkgroup.length; i++) {
					if (document.search_form.checkgroup[i].checked == true) {
						url = url + document.search_form.checkgroup[i].value + ",";
					}
				}
			}
		}


		// Change the the browser location to the new url
		window.location.href = url.replace('m=hrb', 'm=hrd');
	}
}

// Basic search
function basic_search_sobekcm(root, browseurl) {
    // Collect and trim the users's search string
    var term = trimString(document.search_form.u_search.value).replace(",", " ");

    // Look for the MIMETYPE filter (on basic type search )
    if ((document.getElementById('sbkBsav_mimetypeCheck') != null) && (document.getElementById('sbkBsav_mimetypeCheck').checked))
    {
        term = 'NONE,' + term;
        var fieldsm = '-MI,ZZ';

        // Build the destination url by placing the selection option first
        var urlm = root + "?t=" + term + "&f=" + fieldsm;
        if (root.indexOf("?") > 0)
            urlm = root + "&t=" + term + "&f=" + fieldsm;

        window.location.href = urlm;
        return;
    }

    // Look for the MIMETYPE filter ( on rotating home search )
    if ((document.getElementById('sbkRhav_mimetypeCheck') != null) && (document.getElementById('sbkRhav_mimetypeCheck').checked)) {
        term = 'NONE,' + term;
        var fieldsm = '-MI,ZZ';

        // Build the destination url by placing the selection option first
        var urlm = root + "?t=" + term + "&f=" + fieldsm;
        if (root.indexOf("?") > 0)
            urlm = root + "&t=" + term + "&f=" + fieldsm;

        window.location.href = urlm;
        return;
    }
    

	if ((term.length > 0) && (term != '*')) {

		if (term.toLowerCase() == "floridaxx") {
			alert('Please narrow your search by entering additional search terms.');
		}
		else {
			// Show the progress spinner
			var circular_div = document.getElementById("circular_progress");
			if (circular_div != null) {
				circular_div.className = "shown_progress_gray";
			}

			// replace ' or ' and ' and ' and ' not ' in the query
			// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
			term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

			// Build the destination url by placing the selection option first and redirect
			var url = root + "?t=" + term;
		    if (root.indexOf("?") > 0)
		        url = root + "&t=" + term;

			window.location.href = url;
		}
	}
	else if (browseurl.length > 0) {
		window.location.href = browseurl;
	}
}


// Basic search in an aggregation with children selectable
function basic_select_search_sobekcm(root, next_level) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.u_search.value).replace(",", " ");
	if (term.length > 0) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

		// Build the destination url by placing the selection option first
		var url = root + "?t=" + term;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term;

		// Do additional work if there are checkboxes on this form
		if (document.search_form.checkgroup.length > 0) {
			// Check to see if any of the checkboxes are NOT checked
			var test = "true";
			for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
				if (document.search_form.checkgroup[i].checked == false) {
					test = "false";
				}
			}

			// If some were unchecked, include the codes for the checked boxes
			if (test == "false") {
				url = url + "&" + next_level + ".";

				for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
					if (document.search_form.checkgroup[i].checked == true) {
						url = url + document.search_form.checkgroup[i].value + ",";
					}
				}
			}
		}


		// Change the the browser location to the new url
		window.location.href = url.replace('m=hrb', 'm=hrd');
	}
}

// The basic search with year range limiting as well
function basic_search_years_sobekcm(root, browseurl) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.u_search.value).replace(",", " ");

	// Get the year range data first
	var year1 = document.search_form.YearDropDown1.value;
	var year2 = document.search_form.YearDropDown2.value;
	var year_url_append = '';
	if ((year1.length > 0) && (year1 != 'ZZ')) {
		year_url_append = "&yr1=" + year1;

		if ((year2.length > 0) && (year2 != 'ZZ')) {
			year_url_append = year_url_append + "&yr2=" + year2;
		} else {
			year_url_append = year_url_append + "&yr2=" + new Date().getFullYear();
		}
	}


	if ((term.length > 0) && (term != '*')) {

		if (term.toLowerCase() == "floridaxx") {
			alert('Please narrow your search by entering additional search terms.');
		}
		else {
			// Show the progress spinner
			var circular_div = document.getElementById("circular_progress");
			if (circular_div != null) {
				circular_div.className = "shown_progress_gray";
			}

			// replace ' or ' and ' and ' and ' not ' in the query
			// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
			term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

			// Build the destination url by placing the selection option first and redirect
			var url = root + "?t=" + term + year_url_append;
			if (root.indexOf("?") > 0)
				url = root + "&t=" + term + year_url_append;

			window.location.href = url;
		}
	}
	else if (browseurl.length > 0) {
		window.location.href = browseurl;
	}
}

// dLOC Search
function dloc_search_sobekcm(root, browseurl) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.u_search.value).replace(",", " ");
	if ((term.length > 0) && (term != '*')) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

		// Build the destination url by placing the selection option first and redirect
		if (document.search_form.newscheck.checked == false) {

			// determine the base url
			var url = root + "?t=" + term;
			if (root.indexOf("?") > 0)
				url = root + "&t=" + term;

			url = url + ",newspaper&f=TX,-FC";
			window.location.href = url;
		}
		else {
			// determine the base url
			var url = root + "?text=" + term;
			if (root.indexOf("?") > 0)
				url = root + "&text=" + term;

			window.location.href = url;
		}
	}
	else if (browseurl.length > 0) {
		window.location.href = browseurl;
	}
}

// dLOC search in an aggregation with children selectable
function dloc_select_search_sobekcm(root, next_level) {
	// Collect and trim the users's search string
	var term = trimString(document.search_form.u_search.value).replace(",", " ");
	if (term.length > 0) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

		// determine the base url
		var url = root + "?t=" + term;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term;

		// Do additional work if there are checkboxes on this form
		if (document.search_form.checkgroup.length > 0) {
			// Check to see if any of the checkboxes are NOT checked
			var test = "true";
			for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
				if (document.search_form.checkgroup[i].checked == false) {
					test = "false";
				}
			}

			// If some were unchecked, include the codes for the checked boxes
			if (test == "false") {
				url = url + "&" + next_level + ".";

				for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
					if (document.search_form.checkgroup[i].checked == true) {
						url = url + document.search_form.checkgroup[i].value + ",";
					}
				}
			}
		}


		// Build the destination url by placing the selection option first and redirect
		if (document.search_form.newscheck.checked == false) {
			if (document.search_form.textcheck.checked == false) {
				window.location.href = url.replace('m=hrb', 'm=hra').replace('m=lrb', 'm=lra');
			}
			else {
				window.location.href = url.replace('m=hrb', 'm=hra').replace('m=lrb', 'm=lra');
			}
		}
		else {
			if (document.search_form.textcheck.checked == false) {
				window.location.href = url.replace('m=hrb', 'm=hrd').replace('m=lrb', 'm=lrd');
			}
			else {
				window.location.href = url;
			}
		}
	}
}

// Newspaper search
function newspaper_search_sobekcm(root, browseurl) {
	// Collect and trim the users's search string
	if (document.search_form.Dropdownlist1.value == "PP") {
		var term = trimString(document.search_form.Textbox1.value).replace(",", " ") + "," + trimString(document.search_form.Textbox1.value).replace(",", " ");
		var fields = "PP,=SP";
	}
	else {
		var term = trimString(document.search_form.Textbox1.value).replace(",", " ");
		var fields = document.search_form.Dropdownlist1.value;
	}

	if ((term.length > 0) && (term != '*')) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

		var url = root + "?t=" + term + "&f=" + fields;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term + "&f=" + fields;

		// Change the the browser location to the new url
		window.location.href = url;
	}
	else if (browseurl.length > 0) {
		window.location.href = browseurl;
	}
}

// Newspaper search in an aggregation with children selectable
function newspaper_select_search_sobekcm(root, next_level) {
	// Collect and trim the users's search string
	if (document.search_form.Dropdownlist1.value == "PP") {
		var term = trimString(document.search_form.Textbox1.value).replace(",", " ") + "," + trimString(document.search_form.Textbox1.value).replace(",", " ");
		var fields = "PP,=SP";
	}
	else {
		var term = trimString(document.search_form.Textbox1.value).replace(",", " ");
		var fields = document.search_form.Dropdownlist1.value;
	}

	if (term.length > 0) {
		// Show the progress spinner
		var circular_div = document.getElementById("circular_progress");
		circular_div.className = "shown_progress_gray";

		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
		term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

		// URL
		var url = root + "?t=" + term + "&f=" + fields;
		if (root.indexOf("?") > 0)
			url = root + "&t=" + term + "&f=" + fields;

		// Do additional work if there are checkboxes on this form
		if (document.search_form.checkgroup.length > 0) {
			// Check to see if any of the checkboxes are NOT checked
			var test = true;
			for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
				if (document.search_form.checkgroup[i].checked == false) {
					test = false;
				}
			}

			// If some were unchecked, include the codes for the checked boxes
			var found = 0;
			if (test == false) {
				if (next_level.indexOf("s") >= 0) {
					url = url + "&" + next_level + "-";

					for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
						if (document.search_form.checkgroup[i].checked == false) {
							if (found > 0) {
								url = url + "," + document.search_form.checkgroup[i].value;
							}
							else {
								url = url + document.search_form.checkgroup[i].value;
							}
							found++;
						}
					}
				}
				else {
					url = url + "&" + next_level + ".";

					for (i = 0 ; i < document.search_form.checkgroup.length ; i++) {
						if (document.search_form.checkgroup[i].checked == true) {
							url = url + document.search_form.checkgroup[i].value + ",";
						}
					}
				}
			}
		}

		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Trim the input string from the search box
function trimString(str) {
	str = this != window ? this : str;
	return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}

// Functions related to the standard pop-up forms used throughout the application
// and the standard forms readily available to most users

// Code used for the pop up forms
function toggle(div_id) {
	var el = document.getElementById(div_id);
	if (el != null) {
		if (el.style.display == 'none') { el.style.display = 'block'; }
		else { el.style.display = 'none'; }
	}
}

function blanket_size(popUpDivVar, linkname, windowheight) {
    var blanket_height;

	if (typeof window.innerWidth != 'undefined') {
		viewportheight = window.innerHeight;
	} else {
		viewportheight = document.documentElement.clientHeight;
	}
	if ((viewportheight > document.body.parentNode.scrollHeight) && (viewportheight > document.body.parentNode.clientHeight)) {
		blanket_height = viewportheight;
	} else {
		if (document.body.parentNode.clientHeight > document.body.parentNode.scrollHeight) {
			blanket_height = document.body.parentNode.clientHeight;
		} else {
			blanket_height = document.body.parentNode.scrollHeight;
		}
	}

	blanket_height = blanket_height + 100;
	var blanket = document.getElementById('blanket_outer');
	if (blanket != null)
		blanket.style.height = blanket_height + 'px';


	//popUpDiv_height=350; // blanket_height/2-150;//150 is half popup's height
	//popUpDiv.style.top = popUpDiv_height + 'px';

	var popUpDiv = document.getElementById(popUpDivVar);

	if (linkname != null) {
	    var link = document.getElementById(linkname);
	    var maybe_top = findTop(link) - (windowheight / 2);
	    if (maybe_top < 0)
	        maybe_top = 50;
	    if ((maybe_top + windowheight / 2) > blanket_height)
	        maybe_top = blanket_height - windowheight - 50;
	    popUpDiv.style.top = maybe_top + 'px';
	} else {
	    popUpDiv.style.top = '100px';
	}
}

function blanket_size(popUpDivVar, windowheight) {
	if (typeof window.innerWidth != 'undefined') {
		viewportheight = window.innerHeight;
	} else {
		viewportheight = document.documentElement.clientHeight;
	}
	if ((viewportheight > document.body.parentNode.scrollHeight) && (viewportheight > document.body.parentNode.clientHeight)) {
		blanket_height = viewportheight;
	} else {
		if (document.body.parentNode.clientHeight > document.body.parentNode.scrollHeight) {
			blanket_height = document.body.parentNode.clientHeight;
		} else {
			blanket_height = document.body.parentNode.scrollHeight;
		}
	}

	blanket_height = blanket_height + 100;
	var blanket = document.getElementById('blanket_outer');
	if (blanket != null)
		blanket.style.height = blanket_height + 'px';


	//popUpDiv_height=350; // blanket_height/2-150;//150 is half popup's height
	//popUpDiv.style.top = popUpDiv_height + 'px';

	var popUpDiv = jQuery('#' + popUpDivVar);

	// Set the correct top
	var popUpHeight = popUpDiv.css("height").replace("px", "");
    var windowheight2 = jQuery(window).height();

    if (popUpHeight > 0) {
        var div_top_pos = Math.floor((windowheight2 - popUpHeight) / 2);
        popUpDiv.css({ top: div_top_pos + 'px' });
    } else {
        popUpDiv.css({ top: '0px' });
    }

    // Set the correct left
	var popUpWidth = popUpDiv.width();
	var windowwidth = jQuery(window).width();
    var div_left_pos = Math.floor((windowwidth - popUpWidth) / 2) - Math.floor(popUpWidth / 2);
    popUpDiv.css({ left: div_left_pos + 'px' });
}

function window_pos(popUpDivVar, windowwidth) {
	if (typeof window.innerWidth != 'undefined') {
		viewportwidth = window.innerHeight;
	} else {
		viewportwidth = document.documentElement.clientHeight;
	}
	if ((viewportwidth > document.body.parentNode.scrollWidth) && (viewportwidth > document.body.parentNode.clientWidth)) {
		window_left = viewportwidth;
	} else {
		if (document.body.parentNode.clientWidth > document.body.parentNode.scrollWidth) {
			window_left = document.body.parentNode.clientWidth;
		} else {
			window_left = document.body.parentNode.scrollWidth;
		}
	}
	var popUpDiv = document.getElementById(popUpDivVar);
	window_left = (window_left / 2) - (windowwidth / 2);
	popUpDiv.style.left = window_left + 'px';
}


function popup_keypress_focus(windowname, linkname, focusname, windowheight, windowwidth, isMozilla) {
	if (isMozilla == 'False') {
		popup(windowname);
		//blanket_size(windowname, linkname, windowheight );
		//window_pos(windowname, windowwidth);
		//toggle('blanket_outer');
		//toggle(windowname);	
	}
	else {
		var theKeyPressed = evt.charCode || evt.keyCode;
		if (theKeyPressed != 9) {
			popup(windowname);
			//blanket_size(windowname, linkname, windowheight );
			//window_pos(windowname, windowwidth);
			//toggle('blanket_outer');
			//toggle(windowname);	
		}
	}

	// Create the draggable object to allow this window to be dragged around
	//document.getElementById(windowname).draggable();
	$('#' + windowname).draggable();
	//mydrag = new Draggable( windowname, {starteffect:null});

	var focusfield = document.getElementById(focusname);
	focusfield.focus();

	return false;
}

function popup_keypress(windowname, linkname, windowheight, windowwidth, isMozilla) {
	if (isMozilla == 'False') {
		popup(windowname);
	}
	else {
		var theKeyPressed = evt.charCode || evt.keyCode;
		if (theKeyPressed != 9) {
			popup(windowname);
		}
	}

	return false;
}

function popup_keypress(windowname, linkname, isMozilla) {
	if (isMozilla == 'False') {
		popup(windowname);
	}
	else {
		var theKeyPressed = evt.charCode || evt.keyCode;
		if (theKeyPressed != 9) {
			popup(windowname);
		}
	}

	return false;
}

function popup_focus(windowname, linkname, focusname, windowheight, windowwidth) {

	popup(windowname);

	var focusfield = document.getElementById(focusname);
	focusfield.focus();

	return false;
}

function popup_focus(windowname, focusname ) {

	popup(windowname);

	var focusfield = document.getElementById(focusname);
	focusfield.focus();

	return false;
}

function popup(windowname, linkname, windowheight, windowwidth) {
	popup(windowname);
	//blanket_size(windowname, linkname, windowheight );
	//window_pos(windowname, windowwidth);
	//toggle('blanket_outer');
	//toggle(windowname);

	// Create the draggable object to allow this window to be dragged around
	//document.getElementById(windowname).draggable();
	//$('#' + windowname).draggable();
	//mydrag = new Draggable( windowname, {starteffect:null});

	return false;
}

function popup(windowname, popUpHeight, popUpWidth) {

    var popUpDiv = jQuery('#' + windowname);
    blanket_size(windowname, popUpHeight);

    // Set the correct top
    var windowheight2 = jQuery(window).height();
    popUpDiv.css({ top: ((windowheight2 - popUpHeight) / 2) + 'px' });

    // Set the correct left
    var windowwidth = jQuery(window).width();
    popUpDiv.css({ left: ((windowwidth - popUpWidth) / 2) + 'px' });

    toggle('blanket_outer');
    toggle(windowname);

    // Create the draggable object to allow this window to be dragged around
    $('#' + windowname).draggable();

    return false;
}

function popup(windowname) {
	
	var popUpDiv = jQuery('#' + windowname);
	
	// Get the size of the popup
	var windowheight = $('#' + windowname).height();

	blanket_size(windowname, windowheight);

    // Set the correct top
	var popUpHeight = popUpDiv.height();
    if (popUpHeight == 0)
        popUpHeight = 100;
	var windowheight2 = jQuery(window).height();
	popUpDiv.css({ top: ((windowheight2 - popUpHeight) / 2) + 'px' });

    // Set the correct left
	var popUpWidth = popUpDiv.width();
	var windowwidth = jQuery(window).width();
	popUpDiv.css({ left: ((windowwidth - popUpWidth) / 2) + 'px' });

	toggle('blanket_outer');
	toggle(windowname);
	
	// Create the draggable object to allow this window to be dragged around
	$('#' + windowname).draggable();

	return false;
}

function popdown(windowname) {
	toggle('blanket_outer');
	toggle(windowname);

	return false;
}

function findTop(obj) {
	var curtop = 0;
	if ((obj != null) && (obj.offsetParent)) {
		do {
			curtop += obj.offsetTop;
		} while (obj = obj.offsetParent);
	}
	return curtop;
}



function open_new_bookshelf_folder() {

	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'new_bookshelf';

	// Toggle the move form
	blanket_size('new_bookshelf_form', 'new_bookshelf_link', 400);
	window_pos('new_bookshelf_form', 532);
	toggle('blanket_outer');
	toggle('new_bookshelf_form');

	// Create the draggable object to allow this window to be dragged around
	$("#new_bookshelf_form").draggable();

	// Put focus in the right spot
	var focusfield = document.getElementById('new_bookshelf_name');
	focusfield.focus();

	return false;
}

function new_bookshelf_form_close() {
	// Clear the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = '';

	// Close the associated form
	popdown('new_bookshelf_form');

	// Return false to prevent a return trip to the server
	return false;
}

// Populate the interface form and show it
function move_form_open(linkname, id) {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'move';
	var hiddenfield2 = document.getElementById('bookshelf_items');
	hiddenfield2.value = id;

	var legend_div = document.getElementById('move_legend');
	legend_div.innerHTML = 'Select new bookshelf for this item';

	// Toggle the move form
	blanket_size('move_item_form', linkname, 400);
	window_pos('move_item_form', 532);
	toggle('blanket_outer');
	toggle('move_item_form');

	// Create the draggable object to allow this window to be dragged around
	$("#move_item_form").draggable();

	return false;
}

// Form was closed (cancelled) so clear all the data
function move_form_close() {
	// Clear the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = '';
	var hiddenfield2 = document.getElementById('bookshelf_items');
	hiddenfield2.value = '';

	// Close the associated form
	popdown('move_item_form');

	// Return false to prevent a return trip to the server
	return false;
}



function edit_notes_form_open(linkname, id, notes) {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'edit_notes';
	var hiddenfield2 = document.getElementById('bookshelf_items');
	hiddenfield2.value = id;

	var notes_field = document.getElementById('add_notes');
	notes_field.value = notes;

	// Toggle the interface form
	blanket_size('add_item_form', linkname, 400);
	window_pos('add_item_form', 540);
	toggle('blanket_outer');
	toggle('add_item_form');

	// Create the draggable object to allow this window to be dragged around
	$("#add_item_form").draggable();

	// Put focus on the interface code
	var focusfield = document.getElementById('add_notes');
	focusfield.focus();

	return false;
}

// Open the email form
function email_form_open2() {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'email';

	// Pop up the form
	return popup_focus('form_email', 'email_address');
}



// Open the save search/browse form
function save_search_form_open(linkname, id) {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'save_search';

	// Toggle the save search form
	blanket_size('save_search_form', linkname, 90);

	// Position this form differently, since it is buried in the middle of the form
	var popUpDiv = document.getElementById('save_search_form');
	popUpDiv.style.left = '120px';

	toggle('blanket_outer');
	toggle('save_search_form');

	// Create the draggable object to allow this window to be dragged around
	$("#save_search_form").draggable();

	// Put focus on the notes
	var focusfield = document.getElementById('add_notes');
	focusfield.focus();

	return false;
}


// Close the add item form
function save_search_form_close() {
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = '';

    // Close the associated form
    popdown('save_search_form');

    // Return false to prevent a return trip to the server
    return false;
}



function edit_tag(linkname, id, describe_tag) {

	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'add_tag_' + id;

	// Set the value for this tag
	var tagfield = document.getElementById('add_tag');
	tagfield.innerHTML = describe_tag;

	// Toggle the interface form
	blanket_size('describe_item_form', linkname, -36);
	window_pos('describe_item_form', 274);
	toggle('blanket_outer');
	toggle('describe_item_form');

	// Create the draggable object to allow this window to be dragged around
	$("#describe_item_form").draggable();

	// Put focus on the interface code
	var focusfield = document.getElementById('add_tag');
	focusfield.focus();

	return false;
}

// Open descriptive tag form
function describe_item_form_open(linkname, id) {
	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'add_tag';

	// Toggle the interface form
	blanket_size('describe_item_form', linkname, -36);
	window_pos('describe_item_form', 274);
	toggle('blanket_outer');
	toggle('describe_item_form');

	// Create the draggable object to allow this window to be dragged around
	$("#describe_item_form").draggable();

	// Put focus on the interface code
	var focusfield = document.getElementById('add_tag');
	focusfield.focus();

	return false;
}

// Close the add descriptive tag form
function describe_item_form_close() {
	// Clear the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = '';

	// Set the value for this tag
	var tagfield = document.getElementById('add_tag');
	tagfield.innerHTML = '';

	// Close the associated form
	popdown('describe_item_form');

	// Return false to prevent a return trip to the server
	return false;
}



function toggle_share_form2(title, share_url, design_url) {
	var linkname = 'share_button';
	
	// Toggle the share form
	blanket_size('share_form', 90);

	// Position this form differently, since it is buried in the middle of the form
	var popUpDiv = document.getElementById('share_form');
	popUpDiv.style.left = $("#" + linkname).position().left - 100 + "px";
	popUpDiv.style.top = $("#" + linkname).offset().top + "px";
	

	if (popUpDiv.innerHTML.length < 10) {
		popUpDiv.innerHTML = "<a href=\"http://www.facebook.com/share.php?t=" + title + "&amp;u=" + share_url + "\" target=\"FACEBOOK_WINDOW\" onmouseover=\"facebook_share.src=\'" + design_url + "default/images/facebook_share_h.gif\'\" onmouseout=\"facebook_share.src=\'" + design_url + "default/images/facebook_share.gif\'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"facebook_share\" name=\"facebook_share\" src=\"" + design_url + "default/images/facebook_share.gif\" alt=\"FACEBOOK\" /></a>\n" +
			"<a href=\"http://buzz.yahoo.com/buzz?targetUrl=" + share_url + "&amp;headline=" + title + "\" target=\"YAHOOBUZZ_WINDOW\" onmouseover=\"yahoobuzz_share.src='" + design_url + "default/images/yahoobuzz_share_h.gif'\" onmouseout=\"yahoobuzz_share.src='" + design_url + "default/images/yahoobuzz_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoobuzz_share\" name=\"yahoobuzz_share\" src=\"" + design_url + "default/images/yahoobuzz_share.gif\" alt=\"YAHOO BUZZ\" /></a>\n" +
			"<br />" +
			"<a href=\"http://twitter.com/home?status=Currently reading " + share_url + "\" target=\"TWITTER_WINDOW\" onmouseover=\"twitter_share.src='" + design_url + "default/images/twitter_share_h.gif'\" onmouseout=\"twitter_share.src='" + design_url + "default/images/twitter_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"twitter_share\" name=\"twitter_share\" src=\"" + design_url + "default/images/twitter_share.gif\" alt=\"TWITTER\" /></a>" +
			"<a href=\"http://www.google.com/bookmarks/mark?op=add&amp;bkmk=" + share_url + "&amp;title=" + title + "\" target=\"GOOGLE_WINDOW\" onmouseover=\"google_share.src='" + design_url + "default/images/google_share_h.gif'\" onmouseout=\"google_share.src='" + design_url + "default/images/google_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"google_share\" name=\"google_share\" src=\"" + design_url + "default/images/google_share.gif\" alt=\"GOOGLE SHARE\" /></a>" +
			"<br />" +
			"<a href=\"http://www.stumbleupon.com/submit?url=" + share_url + "&amp;title=" + title + "\" target=\"STUMBLEUPON_WINDOW\" onmouseover=\"stumbleupon_share.src='" + design_url + "default/images/stumbleupon_share_h.gif'\" onmouseout=\"stumbleupon_share.src='" + design_url + "default/images/stumbleupon_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"stumbleupon_share\" name=\"stumbleupon_share\" src=\"" + design_url + "default/images/stumbleupon_share.gif\" alt=\"STUMBLEUPON\" /></a>" +
			"<a href=\"http://myweb.yahoo.com/myresults/bookmarklet?t=" + title + "&amp;u=" + share_url + "\" target=\"YAHOO_WINDOW\" onmouseover=\"yahoo_share.src='" + design_url + "default/images/yahoo_share_h.gif'\" onmouseout=\"yahoo_share.src='" + design_url + "default/images/yahoo_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoo_share\" name=\"yahoo_share\" src=\"" + design_url + "default/images/yahoo_share.gif\" alt=\"YAHOO SHARE\" /></a>" +
			"<br />" +
			"<a href=\"http://digg.com/submit?phase=2&amp;url=" + share_url + "&amp;title=" + title + "\" target=\"DIGG_WINDOW\" onmouseover=\"digg_share.src='" + design_url + "default/images/digg_share_h.gif'\" onmouseout=\"digg_share.src='" + design_url + "default/images/digg_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"digg_share\" name=\"digg_share\" src=\"" + design_url + "default/images/digg_share.gif\" alt=\"DIGG\" /></a>" +
			"<a onmouseover=\"favorites_share.src='" + design_url + "default/images/favorites_share_h.gif'\" onmouseout=\"favorites_share.src='" + design_url + "default/images/favorites_share.gif'\" onclick=\"javascript:add_to_favorites();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"favorites_share\" name=\"favorites_share\" src=\"" + design_url + "default/images/favorites_share.gif\" alt=\"MY FAVORITES\" /></a>" +
			"<br />";
	}

	toggle('share_form');

	// Close after five seconds
	setTimeout("close_share_form()", 5000);

	// Return false to prevent a return trip to the server
	return false;
}



function get_viewport() {
	var e = window, a = 'inner';
	if (!('innerWidth' in window)) {
		a = 'client';
		e = document.documentElement || document.body;
	}
	return { width: e[a + 'Width'], height: e[a + 'Height'] };
}


function check_fileextensions_custom(clientid) {

	var fileToUploadElement2 = document.getElementById(clientid + '_fileuploadbox');
	var extField = document.getElementById(clientid + "_allowedExtensions").value;

	var lowered = fileToUploadElement2.value.toLowerCase();

	var ok = true;

	var exts = extField.split(',');

	var valid = false;
	for (var j = 0; j < exts.length; j++) {
		if (lowered.lastIndexOf(exts[j]) != -1) {
			valid = true;
			break;
		}
	}

	ok = ok && valid;

	if (!ok) {
		document.getElementById('invalidExtensionMsg').style.display = 'inline';
		return false;
	}


	else {
		return true;
	}
}


/*** ITEM VIEWER RELATED SCRIPTS **/

// Routines to run after the item viewer is loaded (or resized)
function itemwriter_load() {

	// Below 1000 pixels wide, make the nav bar buttons start at the left
	var viewport_width = get_viewport().width;
	if (viewport_width < 1000) {
		$('.sf-menu').css('margin-left', '5px');
	}
	else {
		$('.sf-menu').css('margin-left', '150px');
	}

	// Get the width and height of the main area
	var main_view_width = 0;
	var main_view_height = 0;
	if ($('#sbkIsw_DocumentDisplay').length == 0) {
		main_view_width = $('#sbkIsw_DocumentDisplay2').width();
		main_view_height = $('#sbkIsw_DocumentDisplay2').height();
	} else {
		main_view_width = $('#sbkIsw_DocumentDisplay').width();
		main_view_height = $('#sbkIsw_DocumentDisplay').height();
	}

	// If the viewport is less than 800 wide, make the nav bar tighter
	if (viewport_width < 950) {
		$('#printbuttonspan').hide();
		$('#addbuttonspan').hide();
		$('#sendbuttonspan').hide();
	}
	else {
		$('#printbuttonspan').show();
		$('#addbuttonspan').show();
		$('#sendbuttonspan').show();
	}


	if ($('#sbkIsw_Leftnavbar').length) {
		// Hide the left column if the size is small enough
		if (document.URL.indexOf('thumbs') < 0) {
			var column_width = $('#sbkIsw_Leftnavbar').width();
			if (viewport_width < 30 + column_width + main_view_width) {

				$('#sbkIsw_Leftnavbar').hide();
				$('#sbkIsw_ShowTocRow').hide();
			} else {
				$('#sbkIsw_Leftnavbar').show();
				$('#sbkIsw_ShowTocRow').show();
			}
		}

		// Set the height of the left nav bar
		if (main_view_height > $('#sbkIsw_Leftnavbar').height())
			$('#sbkIsw_Leftnavbar').css("height", main_view_height);
		else {
			var padding_height = $('#sbkIsw_Leftnavbar').height() - main_view_height;
			$('#sbkIsw_BottomPadding').css("height", padding_height);
		}
	}

	if ($('#sbkIsw_Leftnavbar_hack').length) {
		// Hide the left column if the size is small enough
		if ((document.URL.indexOf('thumbs') < 0) && (main_view_width < 800)) {
			var column_width = $('#sbkIsw_Leftnavbar_hack').width();
			if (viewport_width < 30 + column_width + main_view_width) {
				// alert('viewport_width=' + viewport_width + ', column_width=' + column_width + ', main_view_width=' + main_view_height);
				$('#sbkIsw_Leftnavbar_hack').hide();
				$('#sbkIsw_ShowTocRow').hide();
			} else {
				$('#sbkIsw_Leftnavbar_hack').show();
				$('#sbkIsw_ShowTocRow').show();
			}
		}

		// Set the height of the left nav bar
		if (main_view_height > $('#sbkIsw_Leftnavbar_hack').height())
			$('#sbkIsw_Leftnavbar_hack').css("height", main_view_height);
		else {
			var padding_height = $('#sbkIsw_Leftnavbar_hack').height() - main_view_height;
			$('#sbkIsw_BottomPadding').css("height", padding_height);
		}
	}
}

// Remove an item from the bookshelves from the item viewer
function remove_item_itemviewer() {
	var input_box = confirm("Do you really want to remove this item from all bookshelves?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('item_action');
		hiddenfield.value = 'remove';

		document.itemNavForm.submit();
	}

	// Return false to prevent another return trip to the server
	return false;
}

// Add this document and title to the browser's favorites
function add_to_favorites() {
	var href = location.href;
	var title = document.title;

	if (window.sidebar) {
		window.sidebar.addPanel(title, href, ''); // Mozilla Firefox Bookmark
	}
	else if (window.external) {
		window.external.AddFavorite(href, title); // IE Favorite
	}
	else if (window.opera && window.print) { //Opera Hotlist
		var elem = document.createElement('a'); elem.setAttribute('href', href); elem.setAttribute('title', title); elem.setAttribute('rel', 'sidebar'); elem.click();
	}
	else {
		alert("We are sorry, your browser does not support 'bookmarking' in this manner. You may bookmark using the browsers bookmark button.");
	}

	return false;
}

// Routine used to jump to a specific page in the main item viewer
function item_jump_sobekcm(root) {
	var page_val = document.itemNavForm.page_select.value;
	var url = root.replace("XX1234567890XX", page_val);
	window.location.href = url;
}


// Populate the print form and show it
function print_form_open() {

	// Load the content from the server dynamically first, if currently no content
	var d = $('#form_print');
	if (d.text().trim().length === 0) {
		var toload = document.URL + '?fragment=printform #printform_content';
		if (document.URL.indexOf('?') > 0)
			toload = document.URL + '&fragment=printform #printform_content';
		$('#form_print').load(toload);
	}

	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'print';

    popup('form_print', 500, 500 );

	return false;
}

// Form was closed (cancelled) so clear all the data
function print_form_close() {
	// Clear the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = '';

	// Close the associated form
	popdown('form_print');

	// Return false to prevent a return trip to the server
	return false;
}

// Print the view the user requested
function print_item(page, url_start, url_options) {
	// Close the associated form
	popdown('form_print');

	// CAN NOT USE THE OPTION BELOW SINCE THE RADIO BUTTONS HAVE UNIQUE
	// IDS TO ENABLE THE LABELS TO WORK
	//var print_option = document.itemNavForm.print_pages.value;

	var url_ender = "JJ" + page;

	var include_citation = '0';
	var print_citation_button = document.getElementById('print_citation');
	if (print_citation_button.checked == true)
		include_citation = '1';

	var citation_only_button = document.getElementById('citation_only');
	if (citation_only_button.checked == true)
		url_ender = "FC";

	var contact_sheet_button = document.getElementById('contact_sheet');
	if ((contact_sheet_button != null) && (contact_sheet_button.checked == true))
		url_ender = "RI";

	var all_pages_button = document.getElementById('all_pages');
	if ((all_pages_button != null) && (all_pages_button.checked == true))
		url_ender = "JJ*";

	var tracking_sheet_button = document.getElementById('tracking_sheet');
	if ((tracking_sheet_button != null) && (tracking_sheet_button.checked == true))
		url_ender = "TR";

	var range_button = document.getElementById('range_page');
	if ((range_button != null) && (range_button.checked == true)) {
		var range_from_select = document.getElementById('print_from');
		var range_to_select = document.getElementById('print_to');
		url_ender = "JJ" + range_from_select.value + "-" + range_to_select.value;
	}

	var current_view_button = document.getElementById('current_view');
	if ((current_view_button != null) && (current_view_button.checked = true)) {
		url_ender = "XX" + page;
	}

	// Open new window
	var new_url = url_start + '?options=' + include_citation + url_ender + url_options;
	window.open(new_url, "item_print_window", "status=0,toolbar=0,location=0,menubar=0,directories=0");

	// Return false to prevent a return trip to the server
	return false;
}

// Open the email form
function email_form_open() {

	// Load the content from the server dynamically first, if currently no content
	var d = $('#form_email');
	if (d.text().trim().length === 0) {
		var toload = document.URL + '?fragment=sendform #emailform_content';
		if (document.URL.indexOf('?') > 0)
			toload = document.URL + '&fragment=sendform #emailform_content';
		$('#form_email').load(toload);
	}

	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'email';

	popup('form_email');

	// Put focus on the email address
	var focusfield = document.getElementById('email_address');
	if ( focusfield != null )
		focusfield.focus();

	return false;
}

// Close the email form
function email_form_close() {
	// Clear the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = '';

	// Close the associated form
	popdown('form_email');

	// Return false to prevent a return trip to the server
	return false;
}

// Open add item form
function add_item_form_open() {

	// Load the content from the server dynamically first, if currently no content
	var d = $('#add_item_form');
	if (d.text().trim().length === 0) {
		var toload = document.URL + '?fragment=addform #addform_content';
		if (document.URL.indexOf('?') > 0)
			toload = document.URL + '&fragment=addform #addform_content';
		$('#add_item_form').load(toload);
	}

	// Populate the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = 'add_item';

	// Populate the hidden field with the id
	var hiddenfield2 = document.getElementById('bookshelf_items');
	if (hiddenfield2 != null)
		hiddenfield.value = id;

	popup('add_item_form');

	// Put focus on the notes field
	var focusfield = document.getElementById('add_notes');
	if ( focusfield != null )
		focusfield.focus();

	return false;
}

// Close the add item form
function add_item_form_close() {
	// Clear the hidden value this data
	var hiddenfield = document.getElementById('item_action');
	hiddenfield.value = '';

	// Close the associated form
	popdown('add_item_form');

	// Return false to prevent a return trip to the server
	return false;
}

// Toggle whether the small share form is displayed
function toggle_share_form(linkname) {

	// Load the content from the server dynamically first, if currently no content
	var d = $('#share_form');
	if (d.text().trim().length === 0) {
		var toload = document.URL + '?fragment=shareform #shareform_content';
		if (document.URL.indexOf('?') > 0)
			toload = document.URL + '&fragment=shareform #shareform_content';
		$('#share_form').load(toload);
	}

	// Toggle the interface form
	blanket_size('share_form', linkname, -36);
	window_pos('share_form', -396);
	toggle('share_form');

	// Close after five seconds
	setTimeout("close_share_form()", 5000);

	// Return false to prevent a return trip to the server
	return false;
}

// Close the small share form.. this is done after a delay
function close_share_form() {
	var el = document.getElementById('share_form');
	if (el.style.display == 'block') { el.style.display = 'none'; }
}



// Function to set the full screen mode 
function jp2_set_fullscreen() {
	var x = $("#sbkJp2_Container").offset().left;
	var y = $("#sbkJp2_Container").offset().top;

	var window_height = $(window).height();
	var new_height = window_height - y - 20;
	$("#sbkJp2_Container").height(new_height);

	var window_width = $(window).width();
	var new_width = window_width - x - 30;
	$("#sbkJp2_Container").width(new_width);
}

// Function to set the full screen mode 
function pdf_set_fullscreen(asIframe) {
	var x = $("#sbkPdf_Container").offset().left;
	var y = $("#sbkPdf_Container").offset().top;

	var window_height = $(window).height();
	var new_height = window_height - y - 10;
	$("#sbkPdf_Container").height(new_height);

	var window_width = $(window).width();
	var new_width = window_width - x - 20;
	$("#sbkPdf_Container").width(new_width);

    // Chrome specific
    if ( asIframe == true )
        document.getElementById('sbkPdf_Container').src = document.getElementById('sbkPdf_Container').src;

}


function data_search(root) {
	// Collect and trim the users's search string
	var term = trimString(document.itemNavForm.sbkDvd_SearchBox1.value);
	var field = document.itemNavForm.sbkDvd_Select1.value;

	if (term.length > 0) {

		// Build the destination url by placing the selection option first
		var url = root + "?" + field + "=" + term;
		if (root.indexOf("?") > 0)
			url = root + "&" + field + "=" + term;

		// Change the the browser location to the new url
		window.location.href = url;
	} else {
		window.location.href = root;
	}
}


function dataset_rowselected(this_) {

	var aData = $('#sbkDvd_MainTable').dataTable().fnGetData(this_);
	var key = aData[0];

	var url = window.location.href;
	var new_url = url + "?row=" + key;
	if (url.indexOf('?') > 0)
		new_url = url + "&row=" + key;

	window.location = new_url;
}



/***   RELATED ITEMS SCRIPTS  ****/
//Set the width of the thumbnail and its parent span
function SetParentSpanWidth(imageID, parentID) {
	$ = document;
	var $parent = $.getElementById(parentID);
	var $child = $.getElementById(imageID);
	$child.style.width = $child.offsetWidth + 'px';
	$child.style.display = 'block';

	$parent.style.width = ($child.offsetWidth + 5) + 'px';
	$parent.style.overflow = 'hidden';
	$parent.style.display = 'inline-block';

}
function SetSelectedIndexNT() {
	alert(ntIndex);
	document.getElementById('selectNumOfThumbnails').options[ntIndex].selected = 'true';
}

//Make the span change color and fadeout over 4 seconds

function AddAnchorDivEffect(urlWithAnchor) {
	var spanID = 'span' + urlWithAnchor.substring(urlWithAnchor.indexOf('#') + 1);

	var color = $('.sbkRi_Thumbnail').css('backgroundColor');


	//document.getElementById(spanID).style.background="#FFFF00";
	if (document.getElementById(spanID)) {
		document.getElementById(spanID).className = 'fadeEffect';

		$(".fadeEffect").animate({ backgroundColor: color }, 4000,
            function () {
            	document.getElementById(spanID).className = 'sbkRi_Thumbnail';
            });
	}
}

function AddAnchorDivEffect_QC(urlWithAnchor) {
	var spanID = urlWithAnchor.substring(urlWithAnchor.indexOf('#') + 1);

	var color = $('.thumbnailspan').css('backgroundColor');


	//document.getElementById(spanID).style.background="#FFFF00";
	if (document.getElementById(spanID)) {
		document.getElementById(spanID).className = 'fadeEffect';

		$(".fadeEffect").animate({ backgroundColor: color }, 4000,
            function () {
            	document.getElementById(spanID).className = 'thumbnailspan';
            });
	}
}

//Make the appropriate span change color on pageload, if anchor present in the url
function MakeSpanFlashOnPageLoad() {

}

//Add a search parameter to a url querystring if not already present
//or update it to the value passed-in if present
function UpdateQueryString(key, value, url) {
	if (!url)
		url = window.location.href;
	var re = new RegExp("([?|&])" + key + "=.*?(&|#|$)", "gi");

	if (url.match(re)) {
		if (value)
			return url.replace(re, '$1' + key + "=" + value + '$2');
		else
			return url.replace(re, '$2');
	}
	else {
		if (value) {
			var separator = url.indexOf('?') !== -1 ? '&' : '?',
                hash = url.split('#');
			url = hash[0] + separator + key + '=' + value;
			if (hash[1]) url += '#' + hash[1];
			return url;
		}
		else
			return url;
	}
}

//Check if window has a vertical scrollbar
function HasVerticalScroll() {

	// Check if body height is higher than window height :) 
	if ($("body").height() > $(window).height()) {
		alert("scrollbar present");
		document.getElementById(pageNumbersBottom).style.visibility = 'visible';
		alert("Vertical Scrollbar!");
	}
	else {
		alert("scrollbar not present");
		document.getElementById(pageNumbersBottom).style.visibility = 'hidden';
	}
}

//On Window resize, make the page numbers & buttons at the bottom visible(or not) 
//based on the window size

function WindowResizeActions() {
	//Make page numbers at the bottom of the screen visible only if there is a scrollbar
	$(document).ready(function () {
		$(window).resize(function () {
			// Check if body height is higher than window height 
			if ($("body").height() > $(window).height()) {
				// alert("Window resized!"); 
				var pageSpanID = document.getElementById('pageNumbersBottom');
				pageSpanID.style.visibility = 'visible';

			} else {
				var pageSpanID = document.getElementById('pageNumbersBottom');
				pageSpanID.style.visibility = 'hidden';

			}
		});
	});
}

/** MY SOBEK RELATED SCRIPTS **/
// Delete an item
function delete_item() {
	var hiddenfield = document.getElementById('admin_delete_item');
	hiddenfield.value = "delete";
	document.itemNavForm.submit();
	return false;
}


function close_mysobek_form(FormName) {
	// Close the associated form
	popdown(FormName);

	// Return TRUE to cause a return trip to the server
	return true;
}

// Populate the project form and show it
function popup_mysobek_form(FormName, FocusName) {

	// Toggle the form
	blanket_size(FormName, 215);
	toggle('blanket_outer');
	toggle(FormName);

	// Create the draggable object to allow this window to be dragged around
	$("#" + FormName).draggable();

	// Put focus on the focus element
	if (FocusName.length > 0) {
		var focusfield = document.getElementById(FocusName);
		focusfield.focus();
	}

	return false;
}

function logonTrapKD(event) {

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
		event.returnValue = false;
		event.cancel = true;

		document.itemNavForm.submit();
	}

	return false;
}

/*
 * jQuery Superfish Menu Plugin - v1.7.4
 * Copyright (c) 2013 Joel Birch
 *
 * Dual licensed under the MIT and GPL licenses:
 *	http://www.opensource.org/licenses/mit-license.php
 *	http://www.gnu.org/licenses/gpl.html
 */

; (function ($) {
	"use strict";

	var methods = (function () {
		// private properties and methods go here
		var c = {
			bcClass: 'sf-breadcrumb',
			menuClass: 'sf-js-enabled',
			anchorClass: 'sf-with-ul',
			menuArrowClass: 'sf-arrows'
		},
			ios = (function () {
				var ios = /iPhone|iPad|iPod/i.test(navigator.userAgent);
				if (ios) {
					// iOS clicks only bubble as far as body children
					$(window).load(function () {
						$('body').children().on('click', $.noop);
					});
				}
				return ios;
			})(),
			wp7 = (function () {
				var style = document.documentElement.style;
				return ('behavior' in style && 'fill' in style && /iemobile/i.test(navigator.userAgent));
			})(),
			toggleMenuClasses = function ($menu, o) {
				var classes = c.menuClass;
				if (o.cssArrows) {
					classes += ' ' + c.menuArrowClass;
				}
				$menu.toggleClass(classes);
			},
			setPathToCurrent = function ($menu, o) {
				return $menu.find('li.' + o.pathClass).slice(0, o.pathLevels)
					.addClass(o.hoverClass + ' ' + c.bcClass)
						.filter(function () {
							return ($(this).children(o.popUpSelector).hide().show().length);
						}).removeClass(o.pathClass);
			},
			toggleAnchorClass = function ($li) {
				$li.children('a').toggleClass(c.anchorClass);
			},
			toggleTouchAction = function ($menu) {
				var touchAction = $menu.css('ms-touch-action');
				touchAction = (touchAction === 'pan-y') ? 'auto' : 'pan-y';
				$menu.css('ms-touch-action', touchAction);
			},
			applyHandlers = function ($menu, o) {
				var targets = 'li:has(' + o.popUpSelector + ')';
				if ($.fn.hoverIntent && !o.disableHI) {
					$menu.hoverIntent(over, out, targets);
				}
				else {
					$menu
						.on('mouseenter.superfish', targets, over)
						.on('mouseleave.superfish', targets, out);
				}
				var touchevent = 'MSPointerDown.superfish';
				if (!ios) {
					touchevent += ' touchend.superfish';
				}
				if (wp7) {
					touchevent += ' mousedown.superfish';
				}
				$menu
					.on('focusin.superfish', 'li', over)
					.on('focusout.superfish', 'li', out)
					.on(touchevent, 'a', o, touchHandler);
			},
			touchHandler = function (e) {
				var $this = $(this),
					$ul = $this.siblings(e.data.popUpSelector);

				if ($ul.length > 0 && $ul.is(':hidden')) {
					$this.one('click.superfish', false);
					if (e.type === 'MSPointerDown') {
						$this.trigger('focus');
					} else {
						$.proxy(over, $this.parent('li'))();
					}
				}
			},
			over = function () {
				var $this = $(this),
					o = getOptions($this);
				clearTimeout(o.sfTimer);
				$this.siblings().superfish('hide').end().superfish('show');
			},
			out = function () {
				var $this = $(this),
					o = getOptions($this);
				if (ios) {
					$.proxy(close, $this, o)();
				}
				else {
					clearTimeout(o.sfTimer);
					o.sfTimer = setTimeout($.proxy(close, $this, o), o.delay);
				}
			},
			close = function (o) {
				o.retainPath = ($.inArray(this[0], o.$path) > -1);
				this.superfish('hide');

				if (!this.parents('.' + o.hoverClass).length) {
					o.onIdle.call(getMenu(this));
					if (o.$path.length) {
						$.proxy(over, o.$path)();
					}
				}
			},
			getMenu = function ($el) {
				return $el.closest('.' + c.menuClass);
			},
			getOptions = function ($el) {
				return getMenu($el).data('sf-options');
			};

		return {
			// public methods
			hide: function (instant) {
				if (this.length) {
					var $this = this,
						o = getOptions($this);
					if (!o) {
						return this;
					}
					var not = (o.retainPath === true) ? o.$path : '',
						$ul = $this.find('li.' + o.hoverClass).add(this).not(not).removeClass(o.hoverClass).children(o.popUpSelector),
						speed = o.speedOut;

					if (instant) {
						$ul.show();
						speed = 0;
					}
					o.retainPath = false;
					o.onBeforeHide.call($ul);
					$ul.stop(true, true).animate(o.animationOut, speed, function () {
						var $this = $(this);
						o.onHide.call($this);
					});
				}
				return this;
			},
			show: function () {
				var o = getOptions(this);
				if (!o) {
					return this;
				}
				var $this = this.addClass(o.hoverClass),
					$ul = $this.children(o.popUpSelector);

				o.onBeforeShow.call($ul);
				$ul.stop(true, true).animate(o.animation, o.speed, function () {
					o.onShow.call($ul);
				});
				return this;
			},
			destroy: function () {
				return this.each(function () {
					var $this = $(this),
						o = $this.data('sf-options'),
						$hasPopUp;
					if (!o) {
						return false;
					}
					$hasPopUp = $this.find(o.popUpSelector).parent('li');
					clearTimeout(o.sfTimer);
					toggleMenuClasses($this, o);
					toggleAnchorClass($hasPopUp);
					toggleTouchAction($this);
					// remove event handlers
					$this.off('.superfish').off('.hoverIntent');
					// clear animation's inline display style
					$hasPopUp.children(o.popUpSelector).attr('style', function (i, style) {
						return style.replace(/display[^;]+;?/g, '');
					});
					// reset 'current' path classes
					o.$path.removeClass(o.hoverClass + ' ' + c.bcClass).addClass(o.pathClass);
					$this.find('.' + o.hoverClass).removeClass(o.hoverClass);
					o.onDestroy.call($this);
					$this.removeData('sf-options');
				});
			},
			init: function (op) {
				return this.each(function () {
					var $this = $(this);
					if ($this.data('sf-options')) {
						return false;
					}
					var o = $.extend({}, $.fn.superfish.defaults, op),
						$hasPopUp = $this.find(o.popUpSelector).parent('li');
					o.$path = setPathToCurrent($this, o);

					$this.data('sf-options', o);

					toggleMenuClasses($this, o);
					toggleAnchorClass($hasPopUp);
					toggleTouchAction($this);
					applyHandlers($this, o);

					$hasPopUp.not('.' + c.bcClass).superfish('hide', true);

					o.onInit.call(this);
				});
			}
		};
	})();

	$.fn.superfish = function (method, args) {
		if (methods[method]) {
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		}
		else if (typeof method === 'object' || !method) {
			return methods.init.apply(this, arguments);
		}
		else {
			return $.error('Method ' + method + ' does not exist on jQuery.fn.superfish');
		}
	};

	$.fn.superfish.defaults = {
		popUpSelector: 'ul,.sf-mega', // within menu context
		hoverClass: 'sfHover',
		pathClass: 'overrideThisToUse',
		pathLevels: 1,
		delay: 800,
		animation: { opacity: 'show' },
		animationOut: { opacity: 'hide' },
		speed: 'normal',
		speedOut: 'fast',
		cssArrows: true,
		disableHI: false,
		onInit: $.noop,
		onBeforeShow: $.noop,
		onShow: $.noop,
		onBeforeHide: $.noop,
		onHide: $.noop,
		onIdle: $.noop,
		onDestroy: $.noop
	};

	// soon to be deprecated
	$.fn.extend({
		hideSuperfishUl: methods.hide,
		showSuperfishUl: methods.show
	});

})(jQuery);

/**
 * hoverIntent is similar to jQuery's built-in "hover" method except that
 * instead of firing the handlerIn function immediately, hoverIntent checks
 * to see if the user's mouse has slowed down (beneath the sensitivity
 * threshold) before firing the event. The handlerOut function is only
 * called after a matching handlerIn.
 *
 * hoverIntent r7 // 2013.03.11 // jQuery 1.9.1+
 * http://cherne.net/brian/resources/jquery.hoverIntent.html
 *
 * You may use hoverIntent under the terms of the MIT license. Basically that
 * means you are free to use hoverIntent as long as this header is left intact.
 * Copyright 2007, 2013 Brian Cherne
 *
 * // basic usage ... just like .hover()
 * .hoverIntent( handlerIn, handlerOut )
 * .hoverIntent( handlerInOut )
 *
 * // basic usage ... with event delegation!
 * .hoverIntent( handlerIn, handlerOut, selector )
 * .hoverIntent( handlerInOut, selector )
 *
 * // using a basic configuration object
 * .hoverIntent( config )
 *
 * @param  handlerIn   function OR configuration object
 * @param  handlerOut  function OR selector for delegation OR undefined
 * @param  selector    selector OR undefined
 * @author Brian Cherne <brian(at)cherne(dot)net>
 **/
(function ($) {
	$.fn.hoverIntent = function (handlerIn, handlerOut, selector) {

		// default configuration values
		var cfg = {
			interval: 100,
			sensitivity: 7,
			timeout: 0
		};

		if (typeof handlerIn === "object") {
			cfg = $.extend(cfg, handlerIn);
		} else if ($.isFunction(handlerOut)) {
			cfg = $.extend(cfg, { over: handlerIn, out: handlerOut, selector: selector });
		} else {
			cfg = $.extend(cfg, { over: handlerIn, out: handlerIn, selector: handlerOut });
		}

		// instantiate variables
		// cX, cY = current X and Y position of mouse, updated by mousemove event
		// pX, pY = previous X and Y position of mouse, set by mouseover and polling interval
		var cX, cY, pX, pY;

		// A private function for getting mouse position
		var track = function (ev) {
			cX = ev.pageX;
			cY = ev.pageY;
		};

		// A private function for comparing current and previous mouse position
		var compare = function (ev, ob) {
			ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
			// compare mouse positions to see if they've crossed the threshold
			if ((Math.abs(pX - cX) + Math.abs(pY - cY)) < cfg.sensitivity) {
				$(ob).off("mousemove.hoverIntent", track);
				// set hoverIntent state to true (so mouseOut can be called)
				ob.hoverIntent_s = 1;
				return cfg.over.apply(ob, [ev]);
			} else {
				// set previous coordinates for next time
				pX = cX; pY = cY;
				// use self-calling timeout, guarantees intervals are spaced out properly (avoids JavaScript timer bugs)
				ob.hoverIntent_t = setTimeout(function () { compare(ev, ob); }, cfg.interval);
			}
		};

		// A private function for delaying the mouseOut function
		var delay = function (ev, ob) {
			ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
			ob.hoverIntent_s = 0;
			return cfg.out.apply(ob, [ev]);
		};

		// A private function for handling mouse 'hovering'
		var handleHover = function (e) {
			// copy objects to be passed into t (required for event object to be passed in IE)
			var ev = jQuery.extend({}, e);
			var ob = this;

			// cancel hoverIntent timer if it exists
			if (ob.hoverIntent_t) { ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t); }

			// if e.type == "mouseenter"
			if (e.type == "mouseenter") {
				// set "previous" X and Y position based on initial entry point
				pX = ev.pageX; pY = ev.pageY;
				// update "current" X and Y position based on mousemove
				$(ob).on("mousemove.hoverIntent", track);
				// start polling interval (self-calling timeout) to compare mouse coordinates over time
				if (ob.hoverIntent_s != 1) { ob.hoverIntent_t = setTimeout(function () { compare(ev, ob); }, cfg.interval); }

				// else e.type == "mouseleave"
			} else {
				// unbind expensive mousemove event
				$(ob).off("mousemove.hoverIntent", track);
				// if hoverIntent state is true, then call the mouseOut function after the specified delay
				if (ob.hoverIntent_s == 1) { ob.hoverIntent_t = setTimeout(function () { delay(ev, ob); }, cfg.timeout); }
			}
		};

		// listen for mouseenter and mouseleave
		return this.on({ 'mouseenter.hoverIntent': handleHover, 'mouseleave.hoverIntent': handleHover }, cfg.selector);
	};
})(jQuery);


// Tab code
// Original code by: Matt Walker
// Code source (2013):  http://www.my-html-codes.com/jquery-tabs-my-first-plugin
(function ($) {
	$.fn.acidTabs = function (options) {
		var settings = {
			'style': 'fulltabs'
		};
		options = $.extend(settings, options);
		return this.each(function () {
			var o = options;
			container = this;
			container.setAttribute("class", o.style);
			var navitem = container.querySelector("li");
			//store which tab we are on
			var ident = navitem.id.split("_")[1];
			navitem.parentNode.setAttribute("data-current", ident);
			//set current tab with class of activetabheader
			navitem.setAttribute("class", "tabActiveHeader");

			//hide two tab contents we don't need
			var pages = container.querySelectorAll(".tabpage");
			for (var i = 1; i < pages.length; i++) {
				pages[i].style.display = "none";
			}

			//this adds click event to tabs
			var tabs = container.querySelectorAll("li");
			for (var i = 0; i < tabs.length; i++) {
				tabs[i].onclick = displayPage;
			}
		});

		// on click of one of tabs
		function displayPage() {
			var current = this.parentNode.getAttribute("data-current");
			//remove class of activetabheader and hide old contents
			document.getElementById("tabHeader_" + current).removeAttribute("class");
			document.getElementById("tabpage_" + current).style.display = "none";

			var ident = this.id.split("_")[1];
			//add class of activetabheader to new active tab and show contents
			this.setAttribute("class", "tabActiveHeader");
			document.getElementById("tabpage_" + ident).style.display = "block";
			this.parentNode.setAttribute("data-current", ident);
		}
	};
})(jQuery);

; (function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register module depending on jQuery using requirejs define.
        define(['jquery'], factory);
    } else {
        // No AMD.
        factory(jQuery);
    }
}(function ($) {
    $.fn.addBack = $.fn.addBack || $.fn.andSelf;

    $.fn.extend({

        actual: function (method, options) {
            // check if the jQuery method exist
            if (!this[method]) {
                throw '$.actual => The jQuery method "' + method + '" you called does not exist';
            }

            var defaults = {
                absolute: false,
                clone: false,
                includeMargin: false
            };

            var configs = $.extend(defaults, options);

            var $target = this.eq(0);
            var fix, restore;

            if (configs.clone === true) {
                fix = function () {
                    var style = 'position: absolute !important; top: -1000 !important; ';

                    // this is useful with css3pie
                    $target = $target.
                      clone().
                      attr('style', style).
                      appendTo('body');
                };

                restore = function () {
                    // remove DOM element after getting the width
                    $target.remove();
                };
            } else {
                var tmp = [];
                var style = '';
                var $hidden;

                fix = function () {
                    // get all hidden parents
                    $hidden = $target.parents().addBack().filter(':hidden');
                    style += 'visibility: hidden !important; display: block !important; ';

                    if (configs.absolute === true) style += 'position: absolute !important; ';

                    // save the origin style props
                    // set the hidden el css to be got the actual value later
                    $hidden.each(function () {
                        // Save original style. If no style was set, attr() returns undefined
                        var $this = $(this);
                        var thisStyle = $this.attr('style');

                        tmp.push(thisStyle);
                        // Retain as much of the original style as possible, if there is one
                        $this.attr('style', thisStyle ? thisStyle + ';' + style : style);
                    });
                };

                restore = function () {
                    // restore origin style values
                    $hidden.each(function (i) {
                        var $this = $(this);
                        var _tmp = tmp[i];

                        if (_tmp === undefined) {
                            $this.removeAttr('style');
                        } else {
                            $this.attr('style', _tmp);
                        }
                    });
                };
            }

            fix();
            // get the actual value with user specific methed
            // it can be 'width', 'height', 'outerWidth', 'innerWidth'... etc
            // configs.includeMargin only works for 'outerWidth' and 'outerHeight'
            var actual = /(outer)/.test(method) ?
              $target[method](configs.includeMargin) :
              $target[method]();

            restore();
            // IMPORTANT, this plugin only return the value of the first element
            return actual;
        }
    });
}));