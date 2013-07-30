


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
    if (viewport_width < 890) {
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
            if ((viewport_width < 30 + column_width + main_view_width) && ( main_view_width < 800 )) {
                
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
        if ((document.URL.indexOf('thumbs') < 0) && ( main_view_width < 800 )) {
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
    input_box = confirm("Do you really want to remove this item from all bookshelves?");
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
        $('#form_print').load(toload);
    }

    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'print';

    // Toggle the print form
    blanket_size('form_print', 300);
    window_pos('form_print', 532);
    toggle('blanket_outer');
    toggle('form_print');

    // Create the draggable object to allow this window to be dragged around
    $("#form_print").draggable();

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
        $('#form_email').load(toload);
    }

    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'email';

    // Variable holds the 'height' to use for form placement
    var form_location = 300;
    var horiz_location = 408;

    // Populate the hidden field with the id
    var hiddenfield2 = document.getElementById('bookshelf_items');
    if (hiddenfield2 != null) {
        hiddenfield2.value = id;
        form_location = 664;
        horiz_location = 480;
    }

    // Toggle the email form
    blanket_size('form_email', form_location);
    window_pos('form_email', horiz_location);
    toggle('blanket_outer');
    toggle('form_email');

    // Create the draggable object to allow this window to be dragged around
    $("#form_email").draggable();

    // Put focus on the email address
    var focusfield = document.getElementById('email_address');
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
        $('#add_item_form').load(toload);
    }

    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'add_item';

    // Populate the hidden field with the id
    var hiddenfield2 = document.getElementById('bookshelf_items');
    if (hiddenfield2 != null)
        hiddenfield.value = id;

    // Toggle the add item form
    blanket_size('add_item_form', 300);
    window_pos('add_item_form', 274);
    toggle('blanket_outer');
    toggle('add_item_form');

    // Create the draggable object to allow this window to be dragged around
    $("#add_item_form").draggable();

    // Put focus on the notes field
    var focusfield = document.getElementById('add_notes');
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
    var new_height = window_height - y - 10;
    $("#sbkJp2_Container").height(new_height);

    var window_width = $(window).width();
    var new_width = window_width - x - 20;   
    $("#sbkJp2_Container").width(new_width);
}

// Function to set the full screen mode 
function pdf_set_fullscreen() {
    var x = $("#sbkPdf_Container").offset().left;
    var y = $("#sbkPdf_Container").offset().top;

    var window_height = $(window).height();
    var new_height = window_height - y - 10;
    $("#sbkPdf_Container").height(new_height);

    var window_width = $(window).width();
    var new_width = window_width - x - 20;
    $("#sbkPdf_Container").width(new_width);
}
