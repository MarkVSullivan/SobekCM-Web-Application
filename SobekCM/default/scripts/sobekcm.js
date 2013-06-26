

// Function to add something to the onload event 
function addLoadEvent(func) {
    var oldonload = window.onload;
    if (typeof window.onload != 'function') {
        window.onload = func;
    } else {
        window.onload = function() {
            if (oldonload) {
                oldonload();
            }
            func();
        };
    }
}


// Standard functions used throughout the application
function set_subaggr_display(new_value) {
    var hidden_value = document.getElementById("show_subaggrs");
    if (hidden_value != null) 
    {
        hidden_value.value = new_value;
        document.search_form.submit();
    }

    return false;  
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
    var hidden_value = document.getElementById("internal_header_action");
    hidden_value.value = new_value;
    document.internalHeaderForm.submit();
    return false;
}

// Trim the input string from the search box
function trimString (str) 
{
  str = this != window? this : str;
  return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}

// TEXT BOX ENTER AND LEAVE 
function textbox_enter( elementid, className )
{
    document.getElementById(elementid).className=className;
}

function textbox_leave( elementid, className )
{
    document.getElementById(elementid).className=className;
}

function selectbox_leave( elementid, leaveClassName, initClassName )
{
    var selectElement = document.getElementById(elementid);
    if (( selectElement.value.indexOf('Select') == 0 ) || ( selectElement.value.indexOf('(none)') == 0 ))
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

function Add_Circular_Progress()
{
    var circular_div = document.getElementById("circular_progress");
    circular_div.className = "shown_progress";
}

function focus_element( focusname )
{
    var focusfield = document.getElementById(focusname);
    if ( focusfield != null )
    {
        focusfield.focus();
    }
}


function send_contact_email() {

    // Verify there is either a subject or note
    var notesfield = document.getElementById("notesTextBox");
    var subjectfield = document.getElementById("subjectTextBox");
    if ((notesfield.innerHTML.length == 0) && (subjectfield.value.length == 0))
        return false;
    
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'email';

    document.email_form.submit();
    return false;
}

function remove_aggr_from_myhome( aggregationCode )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'remove_aggregation';
    var hiddenfield2 = document.getElementById('aggregation');
    hiddenfield2.value = aggregationCode;
    
    document.search_form.submit();    
    
    return false;
}

function remove_aggregation()
{
    input_box=confirm("Do you really want to remove this from your personlized home page?");
    if ( input_box == true )
    {
        // Populate the hidden value this data
        var hiddenfield = document.getElementById('item_action');
        hiddenfield.value = 'remove_aggregation';
           
        document.search_form.submit();     
    }
    
    return false;
}

function add_aggregation()
{
    input_box=confirm("Do you really want to add this to your personlized home page?");
    if ( input_box == true )
    {
        // Populate the hidden value this data
        var hiddenfield = document.getElementById('item_action');
        hiddenfield.value = 'add_aggregation';
        
        document.search_form.submit();    
    }
    
    return false;
}

function make_folder_private( foldername )
{
   input_box=confirm("Do you really want to make this folder private?");
    if ( input_box == true )
    {
        // Populate the hidden value this data
        var hiddenfield = document.getElementById('item_action');
        hiddenfield.value = 'private_folder';
        var hiddenfield2 = document.getElementById('aggregation');
        hiddenfield2.value = foldername;
        document.search_form.submit();    
    }
    
    return false;
}


function delete_folder( folder )
{
    input_box=confirm("Do you really want to delete this folder?");
    if ( input_box == true )
    {
        // Populate the hidden value this data
        var hiddenfield = document.getElementById('item_action');
        hiddenfield.value = 'delete_folder';
        var hiddenfield2 = document.getElementById('bookshelf_items');
        hiddenfield2.value = folder;
        
        document.itemNavForm.submit();        
    }
    
    return false;
}

function refresh_bookshelves()
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'refresh_folder';
        
    document.itemNavForm.submit();  
                
    return false;
}

function change_folder_visibility( folder, new_status )
{
    input_box=confirm("Do you really want to make this folder " + new_status + "?");
    if ( input_box == true )
    {
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
function bookshelf_all_check_clicked( items_included )
{
    // Get the number of children
    var item_count = eval( items_included );
    
    // Get this checkbox
    var mainCheckBox = document.getElementById('bookshelf_all_check');
    var mainCheckedValue = mainCheckBox.checked;
    
    for( i = 0 ; i < item_count ; i++ )
    {
        var childCheckBox = document.getElementById('item_select_' + i.toString() );
        childCheckBox.checked = mainCheckedValue;
    }    
}

// Remove a number of items from a bookshelf
function remove_all( items_included )
{
    // Get the number of children
    var item_count = eval( items_included );
    var selected_count = 0;
    var selected_ids = '';
    
    for( i = 0 ; i < item_count ; i++ )
    {
        var childCheckBox = document.getElementById('item_select_' + i.toString() );
        if ( childCheckBox.checked == true )
        {
            selected_count++;
            if ( selected_ids.length > 0 )
                selected_ids = selected_ids + "|" + childCheckBox.value;
            else
                selected_ids = childCheckBox.value;
        }
    } 
    
    var input_box = false;
    if ( selected_count == 0 )
    {
        alert('No items selected!\n\nChoose one or more items by using the checkboxes.');
    }
    else
    {

        if ( selected_count == 1 )
        {
            input_box=confirm("Do you really want to remove this item from this bookshelf?");
        }
        else
        {
            input_box=confirm("Do you really want to remove these " + selected_count + " items from this bookshelf?");
        }
    }
    
    if (input_box==true) 
    { 
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
function move_all( items_included )
{
    // Get the number of children
    var item_count = eval( items_included );
    var selected_count = 0;
    var selected_ids = '';
    
    for( i = 0 ; i < item_count ; i++ )
    {
        var childCheckBox = document.getElementById('item_select_' + i.toString() );
        if ( childCheckBox.checked == true )
        {
            selected_count++;
            if ( selected_ids.length > 0 )
                selected_ids = selected_ids + "|" + childCheckBox.value;
            else
                selected_ids = childCheckBox.value;
        }
    } 
    
    var input_box = true;
    if ( selected_count == 0 )
    {
        alert('No items selected!\n\nChoose one or more items by using the checkboxes.');
        return false;
    }
    else
    {
        if ( selected_count > 1 )
        {
            input_box=confirm("Do you really want to move these " + selected_count + " to another bookshelf?");
        }
    }
    
    if (input_box==true) 
    { 
        // Populate the hidden value this data
        var hiddenfield = document.getElementById('item_action');
        hiddenfield.value = 'move';
        var hiddenfield2 = document.getElementById('bookshelf_items');
        hiddenfield2.value = selected_ids;
        
        // Set the legend correctly
        var legend_div = document.getElementById('move_legend');
        if ( selected_count == 1 )
            legend_div.innerHTML = 'Select new bookshelf for this item';
        else
            legend_div.innerHTML = 'Select new bookshelf for these ' + selected_count + ' items';
         
        // Toggle the move form
	    blanket_size('move_item_form', 'bookshelf_all_move', 400 );
	    window_pos('move_item_form', 532);
	    toggle('blanket_outer');
	    toggle('move_item_form');	
    }
    
    
    // Return false to prevent return trip to the server
    return false;
}

// Remove a search
function delete_search( id )
{
    input_box=confirm("Do you really want to remove this search from your list of saved searches?");
    if (input_box==true) 
    { 
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
function remove_item( id )
{
    input_box=confirm("Do you really want to remove this item from this bookshelf?");
    if (input_box==true) 
    { 
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

function delete_tag( id ) {
    input_box = confirm("Do you really want to remove this descriptive tag?");
    if (input_box == true) {
        // Populate the hidden value this data
        var hiddenfield = document.getElementById('item_action');
        hiddenfield.value = 'delete_tag_' + id;

        document.itemNavForm.submit();
    }

    return false;
}




function preferences( root )
{
    var language_val = document.preferences_form.languageDropDown.value;
    var url = "http://ufdc.ufl.edu/" + root + language_val;
    if ( document.preferences_form.lowRadioButton.Checked )
        url = url + '&ba=s'
	window.location.href = url;
}

// Send the user to the SobekCM search results page for a search within a single item
function item_search_sobekcm( root )
{
	// Collect and trim the users's search string
    var term = trimString(document.itemNavForm.searchTextBox.value);

	if ( term.length > 0 )
	{				
		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
	    term = term.toLowerCase().replace(","," ").replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");
		
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



function sort_results( root )
{
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
function date_jump_sobekcm( root )
{
    // Collect the values from the drop boxes
    var date_result = document.statistics_form.date1_selector.value + document.statistics_form.date2_selector.value;
    var url = root + '/' + date_result;
    window.location.href = url;
}

// Function is used for pulling the statistical information for a new collection
function collection_jump_sobekcm( root )
{
    // Collect the values from the drop boxes
    var collection_selected = document.statistics_form.collection_selector.value;
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
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -"); ;

        var fields = document.internalHeaderForm.internalDropDownList.value;

        // Build the destination url by placing the selection option first
        var url = root + "?t=" + term + "&f=" + fields;
        if (root.indexOf("?") > 0)
            url = root + "&t=" + term + "&f=" + fields;

        // Change the the browser location to the new url
        window.location.href = url;
    }
}