// Functions related to the system wide administrative tasks for logged on system administrators


// Clear the text boxes for a single custom settings
function settings_reorder(selectbox) {
	var startfield = document.getElementById('admin_settings_order');
	startfield.value = selectbox.value;

	var endfield = document.getElementById('admin_settings_action');
	endfield.value = 'reorder';

	// Submit this
	document.itemNavForm.submit();

	return false;
}


// Clear the text boxes for a single custom settings
function clear_setting(key) {
	var startfield = document.getElementById('admin_customkey_' + key);
	startfield.value = '';

	var endfield = document.getElementById('admin_customvalue_' + key);
	endfield.value = '';

	return false;
}

// Verify deletion of the existing project in the database and file
function delete_ip_group(groupid, title) {
    var input_box = confirm("Do you really want to delete project '" + title + "'?");
    if (input_box == true) {
        // Set the hidden value this data
        var hiddenfield = document.getElementById('admin_ip_delete');
        hiddenfield.value = groupid;

        var hiddenfield = document.getElementById('action');
        hiddenfield.value = 'delete';

        document.itemNavForm.submit();
    }

    // Return false to prevent another return trip to the server
    return false;
}


// Clear the text boxes for a single IP address
function clear_ip_address(key) 
{
    var startfield = document.getElementById('admin_ipstart_' + key);
    startfield.value = '';

    var endfield = document.getElementById('admin_ipend_' + key);
    endfield.value = '';

    var labelField = document.getElementById('admin_iplabel_' + key );
    labelField.value = '';

    return false;
}

function save_new_ip_range() {
	// Set the hidden value this data
	var hiddenfield = document.getElementById('action');
	hiddenfield.value = 'new';

	// Return TRUE to force a return trip to the server
	return true;
}

// Reset an aggregation
function reset_aggr( aggr_code )
{
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_aggr_reset');
    hiddenfield.value = aggr_code;
    
    // Submit this
    document.itemNavForm.submit();

    return false;
}

// Save the new buidler status
function save_new_builder_status() {
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_builder_tosave');
    var codefield = document.getElementById('admin_builder_status');
    hiddenfield.value = codefield.value;

    // Return TRUE to force a return trip to the server
    return true;
}

// Save the new aggregation
function save_new_aggr()
{
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_aggr_tosave');
    var codefield = document.getElementById('admin_aggr_code');
    hiddenfield.value = codefield.value;
 
    // Return TRUE to force a return trip to the server
    return true;
}

// Delete an existing aggregation
function delete_aggr(Code) {
	
	var input_box = confirm("Do you really want to delete aggregation '" + Code + "'?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('admin_aggr_delete');
		hiddenfield.value = Code;

		document.itemNavForm.submit();
	}
	
	return false;
}

// Populate the project form and show it
function project_form_popup(code, name, description  ) 
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('admin_project_tosave');
    hiddenfield.value = code;
    
    // Populate the visible fields
    document.getElementById('form_project_code').innerHTML = code + ' &nbsp; &nbsp ';
    document.getElementById('form_project_name').value = name;
    document.getElementById('form_project_desc').value = description;

    // Toggle the project form
    blanket_size('form_project', 215 );
    toggle('blanket_outer');
    toggle('form_project');	
    
    // Create the draggable object to allow this window to be dragged around
    $("#form_project").draggable();
    
    // Put focus on the project name
    var focusfield = document.getElementById('form_project_name');
    focusfield.focus();	
        
    return false;
} 

// Close the project form
function project_form_close()
{
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('admin_project_tosave');
    hiddenfield.value = '';
    
    // Clear the data
    document.getElementById('form_project_code').innerHTML = '';
    document.getElementById('form_project_name').value = '';
    document.getElementById('form_project_desc').value = '';
    
    // Close the associated form
    popdown( 'form_project' );    
    
    // Return false to prevent a return trip to the server
    return false;
}

// Save the new project
function save_new_project()
{
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_project_tosave');
    var codefield = document.getElementById('admin_project_code');
    hiddenfield.value = codefield.value;
 
    // Return TRUE to force a return trip to the server
    return true;
}

// Verify deletion of the existing project in the database and file
function delete_project(code) {
	var input_box = confirm("Do you really want to delete project '" + code + "'?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('admin_project_delete');
		hiddenfield.value = code;

		document.itemNavForm.submit();
	}

	// Return false to prevent another return trip to the server
	return false;
}

// Reset a user's password
function reset_password( id, name )
{
    var input_box=confirm("Do you really want to reset the password for '" + name + "'?");
    if (input_box==true) 
    { 
        // Set the hidden value this data
        var hiddenfield = document.getElementById('admin_user_reset');
        hiddenfield.value = id;
 
        // Submit this
        document.itemNavForm.submit();
    }
	
	// Return false to prevent another return trip to the server
    return false;
}

// Populate the interface form and show it
function interface_form_popup( code, base_code, bannerlink, notes, overrideBanner, overrideHeader, suppressTopNav, buildOnLaunch  ) 
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('admin_interface_tosave');
    hiddenfield.value = code;
    
    // Populate the visible fields
    document.getElementById('form_interface_code').innerHTML = code + ' &nbsp; &nbsp ';
    document.getElementById('form_interface_basecode').value = base_code;
    document.getElementById('form_interface_link').value = bannerlink;
    document.getElementById('form_interface_notes').value = notes;
    
    if ( overrideBanner == 'True' )
        document.getElementById('form_interface_banner_override').checked = true;
    else
        document.getElementById('form_interface_banner_override').checked = false;
        
    if ( overrideHeader == 'True' )
        document.getElementById('form_interface_header_override').checked = true;
    else
        document.getElementById('form_interface_header_override').checked = false;

    if (suppressTopNav == 'True')
        document.getElementById('form_interface_top_nav').checked = true;
    else
        document.getElementById('form_interface_top_nav').checked = false;

    if (buildOnLaunch == 'True')
        document.getElementById('form_interface_buildlaunch').checked = true;
    else
        document.getElementById('form_interface_buildlaunch').checked = false;

    // Toggle the interface form
    blanket_size('form_interface', 215);
    toggle('blanket_outer');
    toggle('form_interface');
            
    // Create the draggable object to allow this window to be dragged around
    $("#form_interface").draggable();
    
    // Put focus on the interface code
    var focusfield = document.getElementById('form_interface_basecode');
    focusfield.focus();	
        
    return false;
} 

// Form was closed (cancelled) so clear all the data
function interface_form_close( )
{
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('admin_interface_tosave');
    hiddenfield.value = '';
    
    // Clear the data
    document.getElementById('form_interface_code').innerHTML = '';
    document.getElementById('form_interface_basecode').value = '';
    document.getElementById('form_interface_link').value = '';
    document.getElementById('form_interface_notes').value = '';
    
    // Close the associated form
    popdown( 'form_interface' );    
    
    // Return false to prevent a return trip to the server
    return false;
}

// Save the new interface
function save_new_interface()
{
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_interface_tosave');
    var codefield = document.getElementById('admin_interface_code');
    hiddenfield.value = codefield.value;
 
    // Return TRUE to force a return trip to the server
    return true;
}

// Sets a hidden value to reset the interface value from the aplication cache
function reset_interface( code )
{
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_interface_reset');
    hiddenfield.value = code;
    
    document.itemNavForm.submit();
    
    // Return false to prevent another return trip to the server
    return false;
}

// Verify deletion of the existing web skin in the database and file
function delete_interface(code) {
	var input_box = confirm("Do you really want to delete web skin '" + code + "'?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('admin_interface_delete');
		hiddenfield.value = code;

		document.itemNavForm.submit();
	}
	
	// Return false to prevent another return trip to the server
	return false;
}

function delete_alias(alias_code) {

    var input_box=confirm("Do you really want to delete the alias '" + alias_code + "'?");
    if (input_box == true) {

        // Populate the hidden value this data
        var hiddenfield = document.getElementById('admin_forwarding_tosave');
        hiddenfield.value = '-' + alias_code;

        document.itemNavForm.submit();
    }
    
    // Return false to prevent another return trip to the server
    return false;
}

// Populate the aggregation alias form and show it
function alias_form_popup(alias, code ) 
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('admin_forwarding_tosave');
    hiddenfield.value = alias;
    
    // Populate the visible fields
    document.getElementById('form_forwarding_alias').innerHTML = alias + ' &nbsp; &nbsp ';
    document.getElementById('form_forwarding_code').value = code;

    // Toggle the interface form
    blanket_size('form_forwarding', 215 );
    toggle('blanket_outer');
    toggle('form_forwarding');	
    
    // Create the draggable object to allow this window to be dragged around
    $("#form_forwarding").draggable();

    // Put focus on the aggregation alias code
    var focusfield = document.getElementById('form_forwarding_code');
    focusfield.focus();	
        
    return false;
} 

// Form was closed (cancelled) so clear all the data
function alias_form_close( )
{
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('admin_forwarding_tosave');
    hiddenfield.value = '';
    
    // Clear the data
    document.getElementById('form_forwarding_alias').innerHTML = '';
    document.getElementById('form_forwarding_code').value = '';
    
    // Close the associated form
    popdown( 'form_forwarding' );    
    
    // Return false to prevent a return trip to the server
    return false;
}

// Save the new alias
function save_new_alias()
{
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_forwarding_tosave');
    var codefield = document.getElementById('admin_forwarding_alias');
    hiddenfield.value = codefield.value; 
   
    // Return TRUE to force a return trip to the server
    return true;
}

function delete_portal(portal_id, portal_name) {

    var input_box = confirm("Do you really want to delete the portal '" + portal_name + "'?");
    if (input_box == true) {

        // Populate the hidden value this data
        var hiddenfield = document.getElementById('admin_portal_tosave');
        hiddenfield.value = portal_id;

        var hiddenfield1 = document.getElementById('admin_portal_action');
        hiddenfield1.value = 'delete';

        document.itemNavForm.submit();
    }

    // Return false to prevent another return trip to the server
    return false;
}



// Populate the url portal form and show it
function portal_form_popup( portal_id, portal_name, portal_abbr, web_skin, aggregation, url_segment, base_purl ) {
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('admin_portal_tosave');
    hiddenfield.value = portal_id;
    var hiddenfield2 = document.getElementById('admin_portal_action');
    hiddenfield2.value = 'edit';

    // Populate the visible fields
    document.getElementById('form_portal_name').value = portal_name;
    document.getElementById('form_portal_abbr').value = portal_abbr;
    document.getElementById('form_portal_skin').value = web_skin;
    document.getElementById('form_portal_skin').text = web_skin;
    document.getElementById('form_portal_aggregation').value = aggregation;
    document.getElementById('form_portal_url').value = url_segment;
    document.getElementById('form_portal_purl').value = base_purl;
	
    if (url_segment.length == 0)
    	document.getElementById("form_portal_url").readOnly = true;
    else
    	document.getElementById("form_portal_url").readOnly = false;

    // Toggle the url portal form
    blanket_size('form_portal', 215);
    toggle('blanket_outer');
    toggle('form_portal');

    // Create the draggable object to allow this window to be dragged around
    $("#form_portal").draggable();

    // Put focus on the portal name field
    var focusfield = document.getElementById('form_portal_name');
    focusfield.focus();

    return false;
}

// Form was closed (cancelled) so clear all the data
function portal_form_close() {
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('admin_portal_tosave');
    hiddenfield.value = '';
    var hiddenfield2 = document.getElementById('admin_portal_action');
    hiddenfield2.value = '';

    // Clear the data
    document.getElementById('form_portal_name').value = '';

    // Close the associated form
    popdown('form_portal');

    // Return false to prevent a return trip to the server
    return false;
}

// Save the new portal
function save_new_portal() {
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_portal_tosave');
    var codefield = document.getElementById('admin_portal_name');
    hiddenfield.value = codefield.value;

    var hiddenfield1 = document.getElementById('admin_portal_action');
    hiddenfield1.value = 'new';

    // Return TRUE to force a return trip to the server
    return true;
}

function delete_heading(theme_id, theme_name) {

    var input_box = confirm("Do you really want to delete the heading '" + theme_name + "'?");
    if (input_box == true) {

        // Populate the hidden value this data
        var hiddenfield = document.getElementById('admin_heading_tosave');
        hiddenfield.value = theme_id;

        var hiddenfield1 = document.getElementById('admin_heading_action');
        hiddenfield1.value = 'delete';

        document.itemNavForm.submit();
    }

    // Return false to prevent another return trip to the server
    return false;
}

// Populate the heading form and show it
function heading_form_popup( theme_name, theme_id) {
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('admin_heading_tosave');
    hiddenfield.value = theme_id;
    var hiddenfield2 = document.getElementById('admin_heading_action');
    hiddenfield2.value = 'edit';

    // Populate the visible fields
    document.getElementById('form_heading_name').value = theme_name;

	// Toggle the hideading form
    blanket_size('form_heading', 215);
    toggle('blanket_outer');
    toggle('form_heading');

	// Create the draggable object to allow this window to be dragged around
    jQuery("#form_heading").draggable();

    // Put focus on the heading name code
    var focusfield = document.getElementById('form_heading_name');
    focusfield.focus();

    return false;
}

// Form was closed (cancelled) so clear all the data
function heading_form_close() {
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('admin_heading_tosave');
    hiddenfield.value = '';
    var hiddenfield2 = document.getElementById('admin_heading_action');
    hiddenfield2.value = '';

    // Clear the data
    document.getElementById('form_heading_name').value = '';

    // Close the associated form
    popdown('form_heading');

    // Return false to prevent a return trip to the server
    return false;
}

// Save the new heading
function save_new_heading() {
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_heading_tosave');
    var codefield = document.getElementById('admin_heading_name');
    hiddenfield.value = codefield.value;

    var hiddenfield1 = document.getElementById('admin_heading_action');
    hiddenfield1.value = 'new';

    // Return TRUE to force a return trip to the server
    return true;
}

// Tell the web app to save all the settings
function admin_settings_save() {
    // Set the hidden value to save all the settings
    var hiddenfield1 = document.getElementById('admin_settings_action');
    hiddenfield1.value = 'save';

    document.itemNavForm.submit();
    
    // Return TRUE to force a return trip to the server
    return true;
}


function move_heading_up( id, current_order ) {
    var hiddenfield1 = document.getElementById('admin_heading_action');
    hiddenfield1.value = 'moveup';

    var hiddenfield2 = document.getElementById('admin_heading_tosave');
    hiddenfield2.value = id + "," + current_order;

    document.itemNavForm.submit();

    // Return false to prevent another return trip to the server
    return false;
}

function move_heading_down(id, current_order) {
    var hiddenfield1 = document.getElementById('admin_heading_action');
    hiddenfield1.value = 'movedown';

    var hiddenfield2 = document.getElementById('admin_heading_tosave');
    hiddenfield2.value = id + "," + current_order;

    document.itemNavForm.submit();

    // Return false to prevent another return trip to the server
    return false;
}

// Populate the wordmark form and show it
function wordmark_form_popup(code, title, file, link ) 
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('admin_wordmark_code_tosave');
    hiddenfield.value = code;

    var hiddenfield2 = document.getElementById('admin_wordmark_action');
    hiddenfield2.value = 'edit';
    
	// Populate the visible fields
    document.getElementById('form_wordmark_code').innerHTML = code + ' &nbsp; &nbsp ';
    document.getElementById('form_wordmark_file').value = file;
    document.getElementById('form_wordmark_title').value = title;
    document.getElementById('form_wordmark_link').value = link;

    // Toggle the wordmark form
    blanket_size('form_wordmark', 215 );
    toggle('blanket_outer');
    toggle('form_wordmark');	
    
    // Create the draggable object to allow this window to be dragged around
    jQuery("#form_wordmark").draggable();
    
    // Put focus on the wordmark file field
    var focusfield = document.getElementById('form_wordmark_file');
    focusfield.focus();	
        
    return false;
} 

// Form was closed (cancelled) so clear all the data
function wordmark_form_close( )
{
    // Clear the hidden value this data
    var hiddenfield = document.getElementById('admin_wordmark_code_tosave');
    hiddenfield.value = '';
    
    // Clear the data
    document.getElementById('form_wordmark_code').innerHTML = '';
    document.getElementById('form_wordmark_file').value = '';
    document.getElementById('form_wordmark_title').value = '';
    document.getElementById('form_wordmark_link').value = '';
    
    // Close the associated form
    popdown( 'form_wordmark' );    
    
    // Return false to prevent a return trip to the server
    return false;
}

// Save the new wordmark
function save_new_wordmark()
{
    // Set the hidden value this data
    var hiddenfield = document.getElementById('admin_wordmark_code_tosave');
    var codefield = document.getElementById('admin_wordmark_code');
    hiddenfield.value = codefield.value;

    var hiddenfield2 = document.getElementById('admin_wordmark_action');
    hiddenfield2.value = 'new';
 
    // Return TRUE to force a return trip to the server
    return true;
}

// Verify deletion of the existing wordmark in the database and file
function delete_wordmark( code )
{
    var input_box=confirm("Do you really want to delete wordmark '" + code + "' and associated file?");
    if (input_box==true) 
    { 
        // Set the hidden value this data
        var hiddenfield = document.getElementById('admin_wordmark_code_delete');
        hiddenfield.value = code;

        var hiddenfield2 = document.getElementById('admin_wordmark_action');
        hiddenfield2.value = 'delete';
 
        document.sbkAdm_AddedForm.submit();
    }
	
	// Return false to prevent another return trip to the server
    return false;
}

// Verify deletion of a wordmark/icon file which is neither referenced in the database nor used
function delete_wordmark_file(code) {
	var input_box = confirm("Do you really want to delete unused image file '" + code + "'?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('admin_wordmark_code_delete');
		hiddenfield.value = code;

		var hiddenfield2 = document.getElementById('admin_wordmark_action');
		hiddenfield2.value = 'delete';

		document.sbkAdm_AddedForm.submit();
	}
	
	// Return false to prevent another return trip to the server
	return false;
}

function wordmark_select_changed(url) {
	var selectField = document.getElementById('admin_wordmark_file');
	var new_url = url + selectField.value;
	jQuery("#sbkWav_SelectedImage").attr("src", new_url);
}



// Save the user updates
function save_user_edits() {
    var hiddenfield = document.getElementById('admin_user_save');
    hiddenfield.value = 'save';
    document.itemNavForm.submit();
    return false;
}

// Save the user updates
function cancel_user_edits() {
    var hiddenfield = document.getElementById('admin_user_save');
    hiddenfield.value = 'cancel';
    document.itemNavForm.submit();
    return false;
}

function new_user_edit_page(page) {
    var hiddenfield = document.getElementById('admin_user_save');
    hiddenfield.value = page;
    document.itemNavForm.submit();
    return false;
}

// Save the user updates
function save_user_group_edits() {
    var hiddenfield = document.getElementById('admin_user_group_save');
    hiddenfield.value = 'save';
    document.itemNavForm.submit();
    return false;
}

// Save the user updates
function cancel_user_group_edits() {
    var hiddenfield = document.getElementById('admin_user_group_save');
    hiddenfield.value = 'cancel';
    document.itemNavForm.submit();
    return false;
}

function new_user_group_edit_page(page) {
    var hiddenfield = document.getElementById('admin_user_group_save');
    hiddenfield.value = page;
    document.itemNavForm.submit();
    return false;
}

// Delete a user group entirely
function delete_user_group(name, id) {
    var input_box = confirm("Do you really want to delete user group '" + name + "'?");
    if (input_box == true) {
        // Set the hidden value this data
        var hiddenfield = document.getElementById('admin_user_group_delete');
        hiddenfield.value = id;

        document.itemNavForm.submit();
    }

    return false;
}


function link_focused2( divname )
{
        var linkspan = document.getElementById(divname);
    if ( linkspan.className.indexOf('_focused') < 0 )
        linkspan.className = linkspan.className.replace('form_linkline_empty ', 'form_linkline_empty_focused ').replace('form_linkline ', 'form_linkline_focused ');
}

function link_blurred2( divname )
{
    var linkspan = document.getElementById(divname);
	if (linkspan.className.indexOf('_focused') > 0)
		linkspan.className = linkspan.className.replace('_focused', '');
}

function link_focused( divname )
{
    var linkspan = document.getElementById(divname);
    if ( linkspan.className.indexOf('_focused') < 0 )
        linkspan.className = linkspan.className + '_focused';
}

function link_blurred( divname )
{
    var linkspan = document.getElementById(divname);
	if (linkspan.className.indexOf('_focused') > 0)
		linkspan.className = linkspan.className.replace('_focused', '');
}

// Save the aggregation updates
function save_aggr_edits() {
    var hiddenfield = document.getElementById('admin_aggr_save');
    hiddenfield.value = 'save';
    document.itemNavForm.submit();
    return false;
}

function new_aggr_edit_page(page) {
    var hiddenfield = document.getElementById('admin_aggr_save');
    hiddenfield.value = page;
    document.itemNavForm.submit();
    return false;
}


function aggr_edit_enable_custom_home() {
    var hiddenfield = document.getElementById('admin_aggr_action');
    hiddenfield.value = 'enable_custom_home';
    document.itemNavForm.submit();
    return false;
}

function aggr_edit_delete_custom_home() {
    var hiddenfield = document.getElementById('admin_aggr_custom_home');
    var strSelect = hiddenfield.options[hiddenfield.selectedIndex].value;
    if (strSelect.length == 0) return false;

    var input_box = confirm("Do you really want to delete the custom home page source file '" + strSelect + "'?");
    if (input_box == true) {
        var hiddenfield = document.getElementById('admin_aggr_action');
        hiddenfield.value = 'delete_custom_home';
        document.itemNavForm.submit();
    }

    // Return false to prevent another return trip to the server
    return false;
}

function aggr_edit_custom_home_selectchange() {
    var hiddenfield = document.getElementById('admin_aggr_custom_home');
    var strSelect = hiddenfield.options[hiddenfield.selectedIndex].value;
    if (strSelect.length > 0) {
        document.getElementById("customHomePageDeleteButton").disabled = false;
    } else {
        document.getElementById("customHomePageDeleteButton").disabled = true;
    }
}

function save_css_edits() {
	var hiddenfield = document.getElementById('admin_aggr_save');
	hiddenfield.value = 'e';
	var hiddenfield2 = document.getElementById('admin_aggr_action');
	hiddenfield2.value = 'save_css';
	document.itemNavForm.submit();
	return false;
}

function aggr_edit_enable_css() {
	var hiddenfield = document.getElementById('admin_aggr_action');
	hiddenfield.value = 'enable_css';
	document.itemNavForm.submit();
	return false;
}

function aggr_edit_disable_css() {
	var hiddenfield = document.getElementById('admin_aggr_action');
	hiddenfield.value = 'disable_css';
	document.itemNavForm.submit();
	return false;
}

function new_aggr_add_home() {
	var hiddenfield = document.getElementById('admin_aggr_action');
	hiddenfield.value = 'add_home';
	document.itemNavForm.submit();
	return false;
}

function aggr_edit_delete_home(code)
{
	var input_box = confirm("Do you really want to delete this home page?");
	if (input_box == true) {
		var hiddenfield = document.getElementById('admin_aggr_action');
		hiddenfield.value = 'delete_home_' + code;
		document.itemNavForm.submit();
	}

	// Return false to prevent another return trip to the server
	return false;
}

function aggr_edit_delete_banner(code, type) {
	var input_box = confirm("Do you really want to delete this banner?");
	if (input_box == true) {
		var hiddenfield = document.getElementById('admin_aggr_action');
		hiddenfield.value = 'delete_' + type + '_' + code;
		document.itemNavForm.submit();
	}

	// Return false to prevent another return trip to the server
	return false;
}

function edit_aggr_banner_select_changed(url) {
	var selectField = document.getElementById('admin_aggr_new_banner_image');
	var new_url = url + selectField.value;
	jQuery("#sbkSaav_SelectedBannerImage").attr("src", new_url);
}

function new_aggr_add_banner() {
	var hiddenfield = document.getElementById('admin_aggr_action');
	hiddenfield.value = 'add_banner';
	document.itemNavForm.submit();
	return false;
}

// Save the new aggregation
function save_new_child_aggr() {
	// Set the hidden value this data
	var hiddenfield = document.getElementById('admin_aggr_action');
	hiddenfield.value = 'save_aggr';
	document.itemNavForm.submit();
	return false;
}


// Delete an existing aggregation
function edit_aggr_delete_child_aggr(Code) {

	var input_box = confirm("Do you really want to delete subcollection '" + Code + "'?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('admin_aggr_action');
		hiddenfield.value = 'delete_' + Code;

		document.itemNavForm.submit();
	}

	return false;
}

function save_new_child_page() {
	// Set the hidden value this data
	var hiddenfield = document.getElementById('admin_aggr_action');
	hiddenfield.value = 'save_childpage';
	document.itemNavForm.submit();
	return false;
}

function edit_aggr_delete_child_page(Code) {
	var input_box = confirm("Do you really want to delete child page '" + Code + "'?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('admin_aggr_action');
		hiddenfield.value = 'delete_' + Code;

		document.itemNavForm.submit();
	}

	return false;
}

function save_new_child_page_version() {
	// Set the hidden value this data
	var hiddenfield = document.getElementById('admin_aggr_action');
	hiddenfield.value = 'add_version';
	document.itemNavForm.submit();
	return false;
}

function aggr_edit_delete_child_version(Language, Code) {
	var input_box = confirm("Do you really want to delete the " + Language + " version?");
	if (input_box == true) {
		// Set the hidden value this data
		var hiddenfield = document.getElementById('admin_aggr_action');
		hiddenfield.value = 'delete_' + Code;

		document.itemNavForm.submit();
	}

	return false;
}

function admin_aggr_child_page_visibility_change() {
	var hiddenfield = document.getElementById('admin_aggr_visibility');
	var hiddenrow = document.getElementById('admin_aggr_parent_row');
	if (hiddenfield.value == 'browse') {
		hiddenrow.style.display = 'table-row';
	} else {
		hiddenrow.style.display = 'none';
	}
}

// Trim the input string from the search box
function trimString (str) 
{
  str = this != window? this : str;
  return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}