// Functions related to the standard pop-up forms used throughout the application
// and the standard forms readily available to most users


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

// Code used for the pop up forms
function toggle(div_id) {
    var el = document.getElementById(div_id);
    if (el != null) {
        if (el.style.display == 'none') { el.style.display = 'block'; }
        else { el.style.display = 'none'; }
    }
}

function blanket_size(popUpDivVar, linkname, windowheight ) {
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
	
	if ( linkname != null )
	{
	    var link = document.getElementById( linkname);
	    maybe_top = findTop( link ) - ( windowheight / 2 );
	    if ( maybe_top < 0 )
	        maybe_top = 50;
	    if (( maybe_top + windowheight / 2 ) > blanket_height )
	        maybe_top = blanket_height - windowheight - 50;
	    popUpDiv.style.top = maybe_top + 'px';
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

    var popUpDiv = document.getElementById(popUpDivVar);
    //maybe_top = ((blanket_height - 100 )/ 2) - windowheight;
    popUpDiv.style.top = 150;
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
	window_left=(window_left/2) - ( windowwidth / 2 );
	popUpDiv.style.left = window_left + 'px';
}


function popup_keypress_focus(windowname, linkname, focusname, windowheight, windowwidth, isMozilla ) 
{
    if ( isMozilla == 'False' )
    {
        blanket_size(windowname, linkname, windowheight );
        window_pos(windowname, windowwidth);
        toggle('blanket_outer');
        toggle(windowname);	
    }
    else
    {
        theKeyPressed = evt.charCode || evt.keyCode;
        if ( theKeyPressed != 9 )
        {    
	        blanket_size(windowname, linkname, windowheight );
	        window_pos(windowname, windowwidth);
	        toggle('blanket_outer');
	        toggle(windowname);		
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

function popup_keypress(windowname, linkname, windowheight, windowwidth, isMozilla ) 
{
    if ( isMozilla == 'False' )
    {
        blanket_size(windowname, linkname, windowheight );
        window_pos(windowname, windowwidth);
        toggle('blanket_outer');
        toggle(windowname);	
    }
    else
    {
        theKeyPressed = evt.charCode || evt.keyCode;
        if ( theKeyPressed != 9 )
        {    
	        blanket_size(windowname, linkname, windowheight );
	        window_pos(windowname, windowwidth);
	        toggle('blanket_outer');
	        toggle(windowname);		
	    }
	}

	// Create the draggable object to allow this window to be dragged around
	//document.getElementById(windowname).draggable();
	$('#' + windowname).draggable();
	//mydrag = new Draggable( windowname, {starteffect:null});
	
	return false;
} 

function popup_focus(windowname, linkname, focusname, windowheight, windowwidth ) {
	blanket_size(windowname, linkname, windowheight );
	window_pos(windowname, windowwidth);
	toggle('blanket_outer');
	toggle(windowname);

	// Create the draggable object to allow this window to be dragged around
	//document.getElementById(windowname).draggable();
	$('#' + windowname).draggable();
	//mydrag = new Draggable( windowname, {starteffect:null});
	
	var focusfield = document.getElementById(focusname);
    focusfield.focus();
    
    
	
		
	return false;
}

function popup(windowname, linkname, windowheight, windowwidth ) {
	blanket_size(windowname, linkname, windowheight );
	window_pos(windowname, windowwidth);
	toggle('blanket_outer');
	toggle(windowname);

	// Create the draggable object to allow this window to be dragged around
	//document.getElementById(windowname).draggable();
	$('#' + windowname).draggable();
	//mydrag = new Draggable( windowname, {starteffect:null});
		    
	return false;
}

function popdown(windowname ) {
	toggle('blanket_outer');
	toggle(windowname);		
		    
	return false;
}

function findTop(obj) {
	var curtop = 0;
    if (( obj != null ) && (obj.offsetParent)) {
        do {
			curtop += obj.offsetTop;
        } while (obj = obj.offsetParent);
    }
	return curtop;
}



function open_new_bookshelf_folder() 
{

    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'new_bookshelf';
        
    // Toggle the move form
	blanket_size('new_bookshelf_form', 'new_bookshelf_link', 400 );
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

function new_bookshelf_form_close()
{
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = '';
       
    // Close the associated form
    popdown( 'new_bookshelf_form' );    
    
    // Return false to prevent a return trip to the server
    return false;
}

// Populate the interface form and show it
function move_form_open(linkname, id  ) 
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'move';
    var hiddenfield2 = document.getElementById('bookshelf_items');
    hiddenfield2.value = id;
    
    var legend_div = document.getElementById('move_legend');
    legend_div.innerHTML = 'Select new bookshelf for this item';
     
    // Toggle the move form
	blanket_size('move_item_form', linkname, 400 );
	window_pos('move_item_form', 532);
	toggle('blanket_outer');
	toggle('move_item_form');	
	
	// Create the draggable object to allow this window to be dragged around
	$("#move_item_form").draggable();
		
	return false;
} 

// Form was closed (cancelled) so clear all the data
function move_form_close( )
{
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = '';
    var hiddenfield2 = document.getElementById('bookshelf_items');
    hiddenfield2.value = '';
       
    // Close the associated form
    popdown( 'move_item_form' );    
    
    // Return false to prevent a return trip to the server
    return false;
}



function edit_notes_form_open( linkname, id, notes )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'edit_notes';
    var hiddenfield2 = document.getElementById('bookshelf_items');
    hiddenfield2.value = id;
    
    var notes_field = document.getElementById('add_notes');
    notes_field.value = notes;    
         
    // Toggle the interface form
	blanket_size('add_item_form', linkname, 400 );
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
function email_form_open2( linkname, id )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'email';
       
    // Toggle the email form
	blanket_size('form_email', linkname, 90 );
	
	// Position this form differently, since it is buried in the middle of the form
	var popUpDiv = document.getElementById('form_email');
	popUpDiv.style.left = '150px';
	
	toggle('blanket_outer');
	toggle('form_email');	
	
	// Create the draggable object to allow this window to be dragged around
	$("#form_email").draggable();
	
	// Put focus on the email address
	var focusfield = document.getElementById('email_address');
    focusfield.focus();	
		
	return false;
}



// Open the save search/browse form
function save_search_form_open( linkname, id )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('item_action');
    hiddenfield.value = 'save_search';
       
    // Toggle the save search form
	blanket_size('add_item_form', linkname, 90 );
	
	// Position this form differently, since it is buried in the middle of the form
	var popUpDiv = document.getElementById('add_item_form');
	popUpDiv.style.left = '120px';
	
	toggle('blanket_outer');
	toggle('add_item_form');	
	
	// Create the draggable object to allow this window to be dragged around
	$("#add_item_form").draggable();
	
	// Put focus on the notes
	var focusfield = document.getElementById('add_notes');
    focusfield.focus();	
		
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



function toggle_share_form2( linkname )
{
    // Toggle the share form
	blanket_size('share_form', linkname, 90 );
	
	// Position this form differently, since it is buried in the middle of the form
	var popUpDiv = document.getElementById('share_form');
	popUpDiv.style.left = '539px';
	
	toggle('share_form');		
	
	// Close after five seconds
	setTimeout( "close_share_form()", 5000 );
	
	// Return false to prevent a return trip to the server
    return false;
}



function get_viewport() 
{
    var e = window, a = 'inner';
    if (!('innerWidth' in window)) 
    {
        a = 'client';
        e = document.documentElement || document.body;
    }
    return { width: e[a + 'Width'], height: e[a + 'Height'] } 
}
