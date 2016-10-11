// Hashtable provides support for those elements which reside on
// one single line
function MyHash() {
    this.length = 0;
    this.items = new Array();
    for (var i = 0; i < arguments.length; i += 2) {
        if (typeof (arguments[i + 1]) != 'undefined') {
            this.items[arguments[i]] = arguments[i + 1];
            this.length++;
        }
    }

    this.removeItem = function(in_key) {
        var tmp_previous;
        if (typeof (this.items[in_key]) != 'undefined') {
            this.length--;
            var tmp_previous = this.items[in_key];
            delete this.items[in_key];
        }

        return tmp_previous;
    }

    this.getItem = function(in_key) {
        return this.items[in_key];
    }

    this.setItem = function(in_key, in_value) {
        var tmp_previous;
        if (typeof (in_value) != 'undefined') {
            if (typeof (this.items[in_key]) == 'undefined') {
                this.length++;
            }
            else {
                tmp_previous = this.items[in_key];
            }

            this.items[in_key] = in_value;
        }

        return tmp_previous;
    }

    this.hasItem = function(in_key) {
        return typeof (this.items[in_key]) != 'undefined';
    }

    this.clear = function() {
        for (var i in this.items) {
            delete this.items[i];
        }

        this.length = 0;
    }
}


// Functions related to the mySobek metadata entry and self-submittal process
var item_count_hash = new MyHash();
var other_title_new_index = 1;
var new_element_index = 1;
var abstract_new_index = 1;
var note_new_index = 1;
var new_language_count = 0;
var new_audience_count = 0;

function addvolume_save_form(further_action) {
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'save' + further_action ;
    // Submit this
    document.itemNavForm.submit();
    return false;
}


function addvolume_cancel_form() {
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'cancel';
    // Submit this
    document.itemNavForm.submit();
    return false;
}

function behaviors_cancel_form() {
    var hiddenfield = document.getElementById('behaviors_request');
    hiddenfield.value = 'cancel';
    // Submit this
    document.itemNavForm.submit();
    return false;
}

function behaviors_save_form() {
    var hiddenfield = document.getElementById('behaviors_request');
    hiddenfield.value = 'save';
    // Submit this
    document.itemNavForm.submit();
    return false;
}

function template_changed() {
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'template';
    var hiddenfield2 = document.getElementById('phase');
    var selectvalue = document.getElementById('prefTemplate');
    hiddenfield2.value = selectvalue.value;
    
    // Submit this
    document.itemNavForm.submit();
    return false;
}

function project_changed() {
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'project';
    var hiddenfield2 = document.getElementById('phase');
    var selectvalue = document.getElementById('prefProject');
    hiddenfield2.value = selectvalue.value;

    // Submit this
    document.itemNavForm.submit();
    return false;
}

// Cancel the request for a new item
function file_delete( filename )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'delete';
    var hiddenfield2 = document.getElementById('phase');
    hiddenfield2.value = filename;
    
    // Submit this
    document.itemNavForm.submit();
    return false;
}

// Cancel the request for a new item
function new_item_cancel()
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'cancel';
    
    // Submit this
    document.itemNavForm.submit();
    return false;
}

// Clear the current item
function new_item_clear()
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'clear';
    
    // Submit this
    document.itemNavForm.submit();
    return false;
}

// Go to a particular phase during new item entry
function new_item_next_phase( next_phase )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'next_phase';
    var hiddenfield2 = document.getElementById('phase');
    hiddenfield2.value = next_phase;
    
    // Submit this
    document.itemNavForm.submit();    
    return false;
}

// Go to a particular phase during new item entry, and save the 
// final value from the ACE Editor into a hidden value in the page before postback
function new_item_next_phase_ace_editor( next_phase )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'next_phase';
    var hiddenfield2 = document.getElementById('phase');
    hiddenfield2.value = next_phase;
	
	// Get the output of the ace editor
	var editor_output = editor.getValue();
	
	// Save the output of the ace editor
	var content_hiddenfield = document.getElementById('tei_source_content');
    content_hiddenfield.value = editor_output;
    
    // Submit this
    document.itemNavForm.submit();    
    return false;
}

// Go to a particular phase during new item entry
function new_upload_next_phase( next_phase )
{
    // Populate the hidden value this data
    var hiddenfield = document.getElementById('action');
    hiddenfield.value = 'next_phase';
    var hiddenfield2 = document.getElementById('phase');
    hiddenfield2.value = next_phase;
    
    // Submit this
    document.itemNavForm.submit();    
    return false;
}

// Clear a text box, by id
function clear_textbox(textboxid) {
    var thisBox = document.getElementById(textboxid);
    if (thisBox != null) {
        thisBox.value = '';
    }
    return false;
}

function combo_text_element_onchange(comboboxid, textboxid, clearFlag) {
    var thisSelectBox = document.getElementById(comboboxid);
    var thisTextBox = document.getElementById(textboxid);
    if ((thisSelectBox != null) && (thisTextBox != null)) {
        var thisValue = thisSelectBox.value;
        if (thisValue.indexOf("|") > 0) {
            thisTextBox.value = thisValue.substring(thisValue.indexOf("|") + 1);
            return false;
        }

        if (clearFlag) {
            thisBox.value = '';
        }
    } else {
        alert('something was null');
    }
    return false;
}

// NEW ELEMENT
function add_new_element_adv( elementName, className )
{
    // Get a reference to the appropriate division
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById( elementName + "_div");
     
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
    
    // Add a new text box
    var newInput = document.createElement('input');    
    newInput.name = id + "new" + new_element_index;
    newInput.type = "text";    
    newInput.id = id + "new" + new_element_index;   
    newInput.className= className + "_input sbk_Focusable";
    thisDiv.appendChild(newInput);
    new_element_index++;  
    
    // Return false to prevent a return trip to the server
    return false;
}

// NEW ELEMENT
function add_new_element( elementName )
{
    // Get a reference to the appropriate division
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById( elementName + "_div");
     
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
    
    // Add a new text box
    var newInput = document.createElement('input');    
    newInput.name = id + "new" + new_element_index;
    newInput.type = "text";    
    newInput.id = id + "new" + new_element_index;   
    newInput.className= elementName + "_input sbk_Focusable";
    thisDiv.appendChild(newInput);
    new_element_index++;  
    
    // Return false to prevent a return trip to the server
    return false;
}

// NEW MULTI TEXT BOX ELEMENT
function add_new_multi_element( elementName, original_count, max_boxes, boxes_per_line )
{
    // Does this already exist in the hash?
    if (!item_count_hash.hasItem(elementName))
        item_count_hash.setItem(elementName, original_count);

    // Check to see if another box should even be possible
    if (max_boxes > 0) {
        if (item_count_hash.getItem(elementName) >= max_boxes)
            return false;
    }

    // Increment the count before adding a new one
    var new_count = item_count_hash.getItem(elementName) + 1;
    item_count_hash.setItem(elementName, new_count);
    
    // Get a reference to the appropriate division
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById(elementName + "_div");

    // Time to go to the next row?
    if (boxes_per_line > 0) {
        if ((item_count_hash.getItem(elementName) % boxes_per_line) == 1) {
            // Add a <br> line here
            var newLine = document.createElement('br');
            thisDiv.appendChild(newLine);
        } 
    }
         
    // Add a new text box
    var newInput = document.createElement('input');
    newInput.name = id + "new" + new_count;
    newInput.type = "text";
    newInput.id = id + "new" + new_count;   
    newInput.className= elementName + "_input sbk_Focusable";
    thisDiv.appendChild(newInput);

    // Should we hide the repeat button now as well?
    if (( max_boxes > 0 ) && (new_count >= max_boxes)) {
        var repeat_button = document.getElementById(elementName + "_repeaticon");
        if (repeat_button != null) {
            repeat_button.style.display = 'none';
        }
    }
    
    // Return false to prevent a return trip to the server
    return false;
}

// NEW MULTI COMBO BOX ELEMENT
function add_new_multi_combo_element(elementName, original_count, max_boxes, boxes_per_line) {
    // Does this already exist in the hash?
    if (!item_count_hash.hasItem(elementName))
        item_count_hash.setItem(elementName, original_count);

    // Check to see if another box should even be possible
    if (max_boxes > 0) {
        if (item_count_hash.getItem(elementName) >= max_boxes)
            return false;
    }

    // Increment the count before adding a new one
    var new_count = item_count_hash.getItem(elementName) + 1;
    item_count_hash.setItem(elementName, new_count);
        
    // Get a reference to the appropriate division
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById(elementName + "_div");

    // Time to go to the next row?
    if (boxes_per_line > 0) {
        if ((item_count_hash.getItem(elementName) % boxes_per_line) == 1) {
            // Add a <br> line here
            var newLine = document.createElement('br');
            thisDiv.appendChild(newLine);
        }
    }
            
    // Add a new combo box
    var newInput = document.createElement('select');
    newInput.name = id + "new" + new_count;
    newInput.id = id + "new" + new_count;   
    newInput.className= elementName + "_input";
    thisDiv.appendChild(newInput);
    
    // Add all the select options
    var templateSelect = document.getElementById(id + '1');
    if ( templateSelect != null )
    {
        for( var z=0; z < templateSelect.length ; z++ )
        {
            var curOp = templateSelect[z].cloneNode(true);
            newInput.appendChild(curOp);
        }
    }

    // Should we hide the repeat button now as well?
    if ((max_boxes > 0) && (new_count >= max_boxes)) {
        var repeat_button = document.getElementById(elementName + "_repeaticon");
        if (repeat_button != null) {
            repeat_button.style.display = 'none';
        }
    }
    
    // Return false to prevent a return trip to the server
    return false;
}

// NEW TWO TEXT BOX ELEMENT
function add_two_text_box_element( elementName, firstLabel, secondLabel )
{
    // Get a reference to the appropriate division
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById( elementName + "_div");
    
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
    
    // Add the first sublabel, if necessary
    if ( firstLabel.length > 0 )
    {
        // Add a new span for the 'scheme' word
        var newFirstLabelSpan = document.createElement('span');  
        newFirstLabelSpan.className= "metadata_sublabel2";
        newFirstLabelSpan.innerHTML=firstLabel + ":";
        thisDiv.appendChild(newFirstLabelSpan);
    }
    
    // Add a new first text box
    var newInput1 = document.createElement('input');    
    newInput1.name = id + "_firstnew" + new_element_index;
    newInput1.type = "text";    
    newInput1.id = id + "_firstnew" + new_element_index;   
    newInput1.className = elementName + "_first_input sbk_Focusable";
    thisDiv.appendChild(newInput1);
    
    // Add the second sublabel, if necessary
    if ( secondLabel.length > 0 )
    {
        // Add a new span for the 'scheme' word
        var newSecondLabelSpan = document.createElement('span');  
        newSecondLabelSpan.className= "metadata_sublabel";
        newSecondLabelSpan.innerHTML=secondLabel + ":";
        thisDiv.appendChild(newSecondLabelSpan);
    }
    
    // Add the new second text box
    var newInput2 = document.createElement('input');    
    newInput2.name = id + "_secondnew" + new_element_index;
    newInput2.type = "text";    
    newInput2.id = id + "_secondnew" + new_element_index;   
    newInput2.className = elementName + "_second_input sbk_Focusable";
    thisDiv.appendChild(newInput2);
    
    new_element_index++;  
    
    // Return false to prevent a return trip to the server
    return false;
}

function container_add_new_item()
{
    var thisDiv = document.getElementById( 'container_div' );
    
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
    
    // Add a new combo box
    var newSelect = document.createElement('select');    
    newSelect.name = "container_selectnew" + new_element_index;
    newSelect.id = "container_selectnew" + new_element_index;   
    newSelect.className= "container_select";
    thisDiv.appendChild(newSelect);    

    // Copy all the options over
    var templateSelect = document.getElementById('container_select1');
    if ( templateSelect != null )
    {
        for( var z=0; z < templateSelect.length ; z++ )
        {
            var curOp = templateSelect[z].cloneNode(true);
            newSelect.appendChild(curOp);
        }
    }        

    
    // Add a new span for the 'label' word
    var newSecondLabelSpan = document.createElement('span');  
    newSecondLabelSpan.className= "metadata_sublabel";
    newSecondLabelSpan.innerHTML="Label:";
    thisDiv.appendChild(newSecondLabelSpan); 
    
    // Add a new text box
    var newInput = document.createElement('input');    
    newInput.name = "container_textnew" + new_element_index;
    newInput.type = "text";    
    newInput.id = "container_textnew" + new_element_index;   
    newInput.className= "container_input sbk_Focusable";
    thisDiv.appendChild(newInput);
    
     new_element_index++;    
    
    // Return false to prevent a return trip to the server
    return false;   
}

// NEW TEXT BOX, SELECT BOX ELEMENT
function add_text_box_select_element( elementName, second_label )
{
    // Get a reference to the appropriate division
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById( elementName + "_div");
     
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
    
    // Add a new text box
    var newInput = document.createElement('input');    
    newInput.name = id + "_textnew" + other_title_new_index;
    newInput.type = "text";    
    newInput.id = id + "_textnew" + other_title_new_index;   
    newInput.className= elementName + "_input sbk_Focusable";
    thisDiv.appendChild(newInput);

    // Add a new span for the 'second_label' if there is one
    if (second_label.length > 0) {
        var newSecondLabelSpan = document.createElement('span');
        newSecondLabelSpan.className = "metadata_sublabel";
        newSecondLabelSpan.innerHTML = " " + second_label + ": ";
        thisDiv.appendChild(newSecondLabelSpan);
    }
    
    // Add a new combo box
    var newSelect = document.createElement('select');    
    newSelect.name = id + "_selectnew" + other_title_new_index;
    newSelect.id = id + "_selectnew" + other_title_new_index;   
    newSelect.className= elementName + "_select";
    thisDiv.appendChild(newSelect);    

    // Copy all the options over
    var templateSelect = document.getElementById(id + '_select1');
    if ( templateSelect != null )
    {
        for( var z=0; z < templateSelect.length ; z++ )
        {
            var curOp = templateSelect[z].cloneNode(true);
            newSelect.appendChild(curOp);
        }
    }        
    other_title_new_index++;     
    
    // Return false to prevent a return trip to the server
    return false;   
}

// NEW DOWNLOAD 
function download_add_new_item()
{
    // Get a reference to the appropriate division
    var elementName = 'download';
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById( elementName + "_div");
     
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
        
    // Add a new combo box
    var newSelect = document.createElement('select');    
    newSelect.name = id + "_selectnew" + new_element_index;
    newSelect.id = id + "_selectnew" + new_element_index;   
    newSelect.className= elementName + "_select";
    thisDiv.appendChild(newSelect);    

    // Copy all the options over
    var templateSelect = document.getElementById(id + '_select1');
    if ( templateSelect != null )
    {
        for( var z=0; z < templateSelect.length ; z++ )
        {
            var curOp = templateSelect[z].cloneNode(true);
            newSelect.appendChild(curOp);
        }
    }
    newSelect.value="";    
    
    // Add a new span for the 'label' word
    var newSecondLabelSpan = document.createElement('span');  
    newSecondLabelSpan.className= "metadata_sublabel";
    newSecondLabelSpan.innerHTML="Label: ";
    thisDiv.appendChild(newSecondLabelSpan); 
    
    // Add a new text box
    var newInput = document.createElement('input');    
    newInput.name = id + "_textnew" + new_element_index;
    newInput.type = "text";    
    newInput.id = id + "_textnew" + new_element_index;   
    newInput.className= elementName + "_input sbk_Focusable";
    thisDiv.appendChild(newInput);
    
    
    new_element_index++;      
    
    // Return false to prevent a return trip to the server
    return false;   
}

// NEW PUBLISHER (OR MANUFACTURER)
function add_publisher_element( elementName )
{
    // Get a reference to the appropriate division
    var id = elementName.replace("_","");
    var thisDiv = document.getElementById( elementName + "_div");
    
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
    
    // Add the name sublabel
    var newFirstLabelSpan = document.createElement('span');  
    newFirstLabelSpan.className= "metadata_sublabel2";
    newFirstLabelSpan.innerHTML= "Name:";
    thisDiv.appendChild(newFirstLabelSpan);
    
    // Add a new first text box
    var newInput1 = document.createElement('input');    
    newInput1.name = id + "_namenew" + new_element_index;
    newInput1.type = "text";    
    newInput1.id = id + "_namenew" + new_element_index;   
    newInput1.className = elementName + "_name_input sbk_Focusable";
    thisDiv.appendChild(newInput1);
    
    // Add a <br> line here
    var newLine = document.createElement('br');   
    thisDiv.appendChild(newLine);
    
    // Add the location sublabel
    var newSecondLabelSpan = document.createElement('span');  
    newSecondLabelSpan.className= "metadata_sublabel2";
    newSecondLabelSpan.innerHTML= "Location(s):";
    thisDiv.appendChild(newSecondLabelSpan);
    
    // Add the first new location text box
    var newInput2 = document.createElement('input');    
    newInput2.name = id + "_firstlocnew" + new_element_index;
    newInput2.type = "text";    
    newInput2.id = id + "_firstlocnew" + new_element_index;   
    newInput2.className = elementName + "_location_input sbk_Focusable";
    thisDiv.appendChild(newInput2);
    
    // Add the second new location text box
    var newInput3 = document.createElement('input');    
    newInput3.name = id + "_secondlocnew" + new_element_index;
    newInput3.type = "text";    
    newInput3.id = id + "_secondlocnew" + new_element_index;   
    newInput3.className = elementName + "_location_input sbk_Focusable";
    thisDiv.appendChild(newInput3);
    
    // Add the third new location text box
    var newInput4 = document.createElement('input');    
    newInput4.name = id + "_thirdlocnew" + new_element_index;
    newInput4.type = "text";    
    newInput4.id = id + "_thirdlocnew" + new_element_index;   
    newInput4.className = elementName + "_location_input sbk_Focusable";
    thisDiv.appendChild(newInput4);
    
    new_element_index++;  
    
    // Return false to prevent a return trip to the server
    return false;
}

// ABSTRACT
function abstract_add_new_item( cols )
{
    // Get a reference to the creator division
    var abstractDiv = document.getElementById("abstract_div");
     
    // Add a <br> line here
    var newLine = document.createElement('br');   
    abstractDiv.appendChild(newLine);
    
    // Add a new text box
    var newInput = document.createElement('textarea');    
    newInput.name = "abstractnew" + abstract_new_index;
    newInput.id = "abstractnew" + abstract_new_index;   
    newInput.className="abstract_input sbk_Focusable";
    newInput.setAttribute("rows", "3");
    newInput.setAttribute("cols", cols );
    abstractDiv.appendChild(newInput);
    abstract_new_index++;   
    
    // Return false to prevent a return trip to the server
    return false; 
}

// NOTE
function note_add_new_item( cols )
{
    // Get a reference to the note division
    var noteDiv = document.getElementById("note_div");
     
    // Add a <br> line here
    var newLine = document.createElement('br');   
    noteDiv.appendChild(newLine);
    
    // Add a new text box
    var newInput = document.createElement('textarea');    
    newInput.name = "notenew" + note_new_index;
    newInput.id = "notenew" + note_new_index;   
    newInput.className="note_input sbk_Focusable";
    newInput.setAttribute("rows", "3");
    newInput.setAttribute("cols", cols );
    noteDiv.appendChild(newInput);
    note_new_index++;   
    
    // Return false to prevent a return trip to the server
    return false; 
}

function add_temporal_element( )
{
    // Get a reference to the temporal division
    var elementName = "complex_temporal";
    var id = elementName.replace("_","");
    var temporal_div = document.getElementById("complex_temporal_div");
    
    // Add a <br> line here
    var newLine = document.createElement('br');   
    temporal_div.appendChild(newLine);
    
    // Add the start year label
    var span1 = document.createElement('span');  
    span1.className= "metadata_sublabel2";
    span1.innerHTML= "Start Year:";
    temporal_div.appendChild(span1);
    
    // Add a start year first text box
    var newInput1 = document.createElement('input');    
    newInput1.name = id + "_startnew" + new_element_index;
    newInput1.type = "text";    
    newInput1.id = id + "_startnew" + new_element_index;   
    newInput1.className = elementName + "_year_input sbk_Focusable";
    temporal_div.appendChild(newInput1);
    
    // Add the end year label
    var span2 = document.createElement('span');  
    span2.className= "metadata_sublabel";
    span2.innerHTML= "End Year:";
    temporal_div.appendChild(span2);
    
    // Add a start year first text box
    var newInput2 = document.createElement('input');    
    newInput2.name = id + "_endnew" + new_element_index;
    newInput2.type = "text";    
    newInput2.id = id + "_endnew" + new_element_index;   
    newInput2.className = elementName + "_year_input sbk_Focusable";
    temporal_div.appendChild(newInput2);
    
    // Add the period label
    var span3 = document.createElement('span');  
    span3.className= "metadata_sublabel";
    span3.innerHTML= "Period:";
    temporal_div.appendChild(span3);
    
    // Add a start year first text box
    var newInput3 = document.createElement('input');    
    newInput3.name = id + "_periodnew" + new_element_index;
    newInput3.type = "text";    
    newInput3.id = id + "_endperiod" + new_element_index;   
    newInput3.className = elementName + "_period_input sbk_Focusable";
    temporal_div.appendChild(newInput3);    
        
    new_element_index++;     
    
    // Return false to prevent a return trip to the server
    return false; 
}

// Special code for the IR Type Element
function ir_type_change( )
{
    // Set the text for the larger body of work
    var ir_select = document.getElementById("irtype");
    var selected_value = ir_select.value;
    var larger_label_div = document.getElementById("larger_body_label");
    var new_label_value = "Larger Body of Work:";
    switch( selected_value )
    {
        case "Book Chapter":
            new_label_value = "Book Title:";
            break;
        
        case "Conference Papers":
        case "Conference Proceedings":
            new_label_value = "Conference Name:";
            break;

        case "Course Material":
            new_label_value = "Course Name:";
            break;
            
        case "Journal Article":
            new_label_value = "Journal Citation:";
            break;
        
        case "Technical Reports":
            new_label_value = "Series Title:";
            break;    
            
        case "Other":
            new_label_value = "Larger Body of Work:";
            break;
    }
    larger_label_div.innerHTML = new_label_value; 
    
    // Select the initial 'Select Material Type' test
    var select_to_remove = -1;
    for(i=0; i < ir_select.options.length; i++)
    {
      if(ir_select.options[i].value == "Select Material Type")
      {
        select_to_remove = i;
      }
    }
    if ( select_to_remove >= 0 )
    {
        ir_select.options[select_to_remove] = null;
    }    
    
    // If this is 'Other' add the additional stuff, otherwise remove it
    var ir_div = document.getElementById("ir_type_div");
    if ( selected_value == 'Other' )
    {
        if ( document.getElementById("irtype_othertext") == null )
        {
            // Add the end year label
            var span = document.createElement('span');  
            span.className= "metadata_sublabel";
            span.innerHTML= "Specify Type:";
            span.id = "irtype_othertext";
            span.name = "irtype_othertext";
            ir_div.appendChild(span);
    
            // Add a start year first text box
            var newInput = document.createElement('input');    
            newInput.name = "irtype_otherinput";
            newInput.type = "text";    
            newInput.id = "irtype_otherinput";  
            newInput.className = "irtype_other_input sbk_Focusable";
            ir_div.appendChild(newInput);  
        }  
    }
    else
    {
        if ( document.getElementById("irtype_othertext") != null )
        {
            // Add the end year label
            var remove_span = document.getElementById("irtype_othertext");
            var remove_input = document.getElementById("irtype_otherinput");
            ir_div.removeChild(remove_span);
            ir_div.removeChild(remove_input);
        }  
    }    
}

function complexnote_type_change( note_index )
{
    // Get a reference to the complex note division and some elements
    var elementName = "complex_note";
    var id = elementName.replace("_","");
    var top_div = document.getElementById( 'complex_note_topdiv' + note_index );
    
    // Get reference to the main combo box and value
    var type_select = document.getElementById( id + "_type" + note_index);
    var selected_value = type_select.value;
    
    switch( selected_value )
    {
        case "500":
        case "511":        
        case "362":
        case "585":
        case "546":
        case "561":
        case "524":
        case "581":
        case "518":
            if ( document.getElementById("complexnote_input" + note_index ) == null )
            {
                // Add the end year label
                var span = document.createElement('span');  
                span.className= "metadata_sublabel";
                span.innerHTML= "Materials Specified:";
                if ( selected_value == "362")
                    span.innerHTML = "Source:";      
                if (( selected_value == "500") || ( selected_value == "511" ))
                    span.innerHTML = "Display Label:";
                span.id = "complexnote_inputtext"  + note_index;
                span.name = "complexnote_inputtext"  + note_index
                top_div.appendChild(span);
        
                // Add a start year first text box
                var newInput = document.createElement('input');    
                newInput.name = "complexnote_input" + note_index;
                newInput.type = "text";    
                newInput.id = "complexnote_input" + note_index;
                newInput.className = "complexnote_input sbk_Focusable";
                top_div.appendChild(newInput);  
            } 
            else
            {
                var text_span = document.getElementById("complexnote_inputtext"  + note_index );
                                
                if (( selected_value == "362") || ( selected_value == "500") || ( selected_value == "511" ))
                {
                    if ( selected_value == "362" )
                        text_span.innerHTML = "Source:";
                    else
                        text_span.innerHTML = "Display Label:";
                }
                else
                    text_span.innerHTML= "Materials Specified:";
            }
            break;
            
        default:
            if ( document.getElementById("complexnote_input" + note_index ) != null )
            {
                var remove_span = document.getElementById("complexnote_inputtext"  + note_index );
                var remove_input = document.getElementById("complexnote_input" + note_index );
                top_div.removeChild(remove_span);
                top_div.removeChild(remove_input);
             } 
            break;    
    
    }
}

function add_complex_note( rows, cols )
{
    // Get a reference to the temporal division
    var elementName = "complex_note";
    var id = elementName.replace("_","");
    var complex_div = document.getElementById("complex_note_div");
    var append_new_note_id = 'new' + abstract_new_index;
    
    // Add a <br> line here
    var newLine = document.createElement('br');   
    complex_div.appendChild(newLine);
    
    // Add the top div
    var top_div = document.createElement('div');
    top_div.id = "complex_note_topdiv" + append_new_note_id;
    complex_div.appendChild( top_div );
    
    // Add the type span
    var typeSpan = document.createElement('span');   
    typeSpan.innerHTML = "Type:";
    typeSpan.className = "metadata_sublabel2";
    top_div.appendChild(typeSpan);

    // Add the select statement
    var type_select = document.createElement('select');
    type_select.className = "complex_note_type";
    type_select.name = "complexnote_type" + append_new_note_id;
    type_select.id = "complexnote_type" + append_new_note_id;  

    // Copy all the options over
    var templateSelect = document.getElementById( 'complexnote_type1');
    if ( templateSelect != null )
    {
        for( var z=0; z < templateSelect.length ; z++ )
        {
            var curOp = templateSelect[z].cloneNode(true);
            type_select.appendChild(curOp);
        }
    } 
    
    // Finish the select and add to the divisino
    type_select.onchange = function() { complexnote_type_change( append_new_note_id ) };
    top_div.appendChild(type_select); 
    
    // Add a new text box
    var newInput = document.createElement('textarea');    
    newInput.name = "complexnote_textarea" + append_new_note_id;
    newInput.id = "complexnote_textarea" + append_new_note_id;   
    newInput.className="complex_note_input sbk_Focusable";
    newInput.setAttribute("rows", rows );
    newInput.setAttribute("cols", cols );
    complex_div.appendChild(newInput);    

    // Increment the counter
    abstract_new_index++;    
    
    // Return false to prevent a return trip to the server
    return false;
}

function add_complex_abstract( rows, cols )
{
    // Get a reference to the temporal division
    var elementName = "complex_abstract";
    var id = elementName.replace("_","");
    var complex_div = document.getElementById("complex_abstract_div");
    var append_new_note_id = 'new' + abstract_new_index;
    
    // Add a <br> line here
    var newLine = document.createElement('br');   
    complex_div.appendChild(newLine);
    
    // Add the top div
    var top_div = document.createElement('div');
    top_div.id = "complex_abstract_topdiv" + append_new_note_id;
    complex_div.appendChild( top_div );
    
    // Add the type span
    var typeSpan = document.createElement('span');   
    typeSpan.innerHTML = "Type:";
    typeSpan.className = "metadata_sublabel2";
    top_div.appendChild(typeSpan);

    // Add the select statement
    var type_select = document.createElement('select');
    type_select.className = "complex_abstract_type";
    type_select.name = "complexabstract_type" + append_new_note_id;
    type_select.id = "complexabstract_type" + append_new_note_id;  


    // Copy all the options over
    var templateSelect = document.getElementById( 'complexabstract_type1');
    if ( templateSelect != null )
    {
        for( var z=0; z < templateSelect.length ; z++ )
        {
            var curOp = templateSelect[z].cloneNode(true);
            type_select.appendChild(curOp);
        }
    } 
    
    // Finish the select and add to the divisino
    top_div.appendChild(type_select); 
    
    // Add the language span
    var languageSpan = document.createElement('span');   
    languageSpan.innerHTML = "Language:";
    languageSpan.className = "metadata_sublabel";
    top_div.appendChild(languageSpan);
    
    // Add the language text box
    var langInput = document.createElement('input');    
    langInput.name = "complexabstract_language" + append_new_note_id;
    langInput.type = "text";    
    langInput.id = "complexabstract_language" + append_new_note_id;
    langInput.className = "complex_abstract_language sbk_Focusable";
    top_div.appendChild(langInput);  
    
    // Add a new text box
    var newInput = document.createElement('textarea');    
    newInput.name = "complexabstract_textarea" + append_new_note_id;
    newInput.id = "complexabstract_textarea" + append_new_note_id;   
    newInput.className="complex_abstract_input sbk_Focusable";
    newInput.setAttribute("rows", rows );
    newInput.setAttribute("cols", cols );
    complex_div.appendChild(newInput);    

    // Increment the counter
    abstract_new_index++;    
    
    // Return false to prevent a return trip to the server
    return false;
}

function add_viewer_element( )
{
    // Get a reference to the temporal division
    var viewer_div = document.getElementById("viewer_div");
    var append_new_id = 'new' + new_element_index;
    
    // Add a <br> line here
    viewer_div.appendChild(document.createElement('br'));
    
    // Add the select statement
    var type_select = document.createElement('select');
    type_select.className = "viewer_type";
    type_select.name = "viewer_type" + append_new_id;
    type_select.id = "viewer_type" + append_new_id;  
    //type_select.onchange = function() { viewer_type_changed(type_select.id) };
    viewer_div.appendChild(type_select);
    
    // Copy all the options over
    var origTypeSelect = document.getElementById( 'viewer_type1');
    if ( origTypeSelect != null )
    {
        for( var z=0; z < origTypeSelect.length ; z++ )
        {
            var curOp = origTypeSelect[z].cloneNode(true);
            type_select.appendChild(curOp);
        }
    } 
    
    /*
    // Add the details span
    var detailsSpan = document.createElement('span');   
    detailsSpan.style.display = "none";
    detailsSpan.id = "viewer_details" + append_new_id;
    viewer_div.appendChild(detailsSpan);
    
    //  Add the file sublabel to the details span
    var fileSpan = document.createElement('span');   
    fileSpan.innerHTML = "File:";
    fileSpan.className = "metadata_sublabel";
    detailsSpan.appendChild(fileSpan);
    
    // Add the file select to the details span
    var file_select = document.createElement('select');
    file_select.className = "viewer_file";
    file_select.name = "viewer_file" + append_new_id;
    file_select.id = "viewer_file" + append_new_id;  
    detailsSpan.appendChild(file_select);
    
    
    // Copy all the options over
    var origFileSelect = document.getElementById( 'viewer_file1');
    if ( origFileSelect != null )
    {
        for( var z=0; z < origFileSelect.length ; z++ )
        {
            var curOp = origFileSelect[z].cloneNode(true);
            file_select.appendChild(curOp);
        }
    } 
    
    //  Add the label sublabel to the details span
    var labelSpan = document.createElement('span');   
    labelSpan.innerHTML = "Label:";
    labelSpan.className = "metadata_sublabel";
    detailsSpan.appendChild(labelSpan);
    
    // Add a new text box
    var newInput = document.createElement('input');    
    newInput.type = "text";    
    newInput.name = "viewer_label" + append_new_id;
    newInput.id = "viewer_label" + append_new_id;   
    newInput.className="viewer_label_input sbk_Focusable";
    detailsSpan.appendChild(newInput);    

    */
    
    // Increment the counter
    new_element_index++;    
    
    // Return false to prevent a return trip to the server
    return false;
}

function close_subject_form( windowname )
{
    diff = windowname.replace('form_subject_','');
    var linkspan = document.getElementById('form_subject_term_' + diff);
    
    new_link_text = '';
    
    var occup1input = document.getElementById('formsubjectoccup1_' + diff );
    if (( occup1input != null ) && ( (trimString(occup1input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(occup1input.value);    

    var topic1input = document.getElementById('formsubjecttopic1_' + diff );
    if (( topic1input != null ) && ( (trimString(topic1input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(topic1input.value);

    var topic2input = document.getElementById('formsubjecttopic2_' + diff );
    if (( topic2input != null ) && ( (trimString(topic2input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(topic2input.value);        
        
    var topic3input = document.getElementById('formsubjecttopic3_' + diff );
    if (( topic3input != null ) && ( (trimString(topic3input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(topic3input.value);
        
    var topic4input = document.getElementById('formsubjecttopic4_' + diff );
    if (( topic4input != null ) && ( (trimString(topic4input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(topic4input.value);
        
    var genre1input = document.getElementById('formsubjectgenre1_' + diff );
    if (( genre1input != null ) && ( (trimString(genre1input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(genre1input.value);
        
    var genre2input = document.getElementById('formsubjectgenre2_' + diff );
    if (( genre2input != null ) && ( (trimString(genre2input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(genre2input.value);
        
    var geo1input = document.getElementById('formsubjectgeo1_' + diff );
    if (( geo1input != null ) && ( (trimString(geo1input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(geo1input.value);
        
    var geo2input = document.getElementById('formsubjectgeo2_' + diff );
    if (( geo2input != null ) && ( (trimString(geo2input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(geo2input.value);
        
    var temporal1input = document.getElementById('formsubjecttemporal1_' + diff );
    if (( temporal1input != null ) && ( (trimString(temporal1input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(temporal1input.value);
        
    var temporal2input = document.getElementById('formsubjecttemporal2_' + diff );
    if (( temporal2input != null ) && ( (trimString(temporal2input.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(temporal2input.value);
        
    if ( new_link_text.length > 0 )
    {
        new_link_text = new_link_text.substring(4);
        
        var authorityinput = document.getElementById('formsubjectauthority_' + diff );
        if (( authorityinput != null ) && ( (trimString(authorityinput.value)).length > 0 ))
            new_link_text = new_link_text + ' ( ' + trimString(authorityinput.value) + ' )';
            
        linkspan.className = 'form_linkline form_subject_line';
    }
    else
    {
        new_link_text = "<i>Empty Subject Keyword</i>";
        linkspan.className = 'form_linkline_empty form_subject_line';
    }
            
    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( windowname );    
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function close_spatial_form( windowname )
{
    diff = windowname.replace('form_spatial_','');
    var linkspan = document.getElementById('form_spatial_term_' + diff);
    
    new_link_text = '';
    
    var continentinput = document.getElementById('formspatialcontinent_' + diff );
    if (( continentinput != null ) && ( (trimString(continentinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(continentinput.value);    

    var countryinput = document.getElementById('formspatialcountry_' + diff );
    if (( countryinput != null ) && ( (trimString(countryinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(countryinput.value);

    var provinceinput = document.getElementById('formspatialprovince_' + diff );
    if (( provinceinput != null ) && ( (trimString(provinceinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(provinceinput.value);        
        
    var regioninput = document.getElementById('formspatialregion_' + diff );
    if (( regioninput != null ) && ( (trimString(regioninput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(regioninput.value);
        
    var stateinput = document.getElementById('formspatialstate_' + diff );
    if (( stateinput != null ) && ( (trimString(stateinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(stateinput.value);
        
    var territoryinput = document.getElementById('formspatialterritory_' + diff );
    if (( territoryinput != null ) && ( (trimString(territoryinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(territoryinput.value);
        
    var countyinput = document.getElementById('formspatialcounty_' + diff );
    if (( countyinput != null ) && ( (trimString(countyinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(countyinput.value);
        
    var cityinput = document.getElementById('formspatialcity_' + diff );
    if (( cityinput != null ) && ( (trimString(cityinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(cityinput.value);
        
    var islandinput = document.getElementById('formspatialisland_' + diff );
    if (( islandinput != null ) && ( (trimString(islandinput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(islandinput.value);
        
    var areainput = document.getElementById('formspatialarea_' + diff );
    if (( areainput != null ) && ( (trimString(areainput.value)).length > 0 ))
        new_link_text = new_link_text + ' -- ' + trimString(areainput.value);
         
    if ( new_link_text.length > 0 )
    {
        new_link_text = new_link_text.substring(4);
        linkspan.className = 'form_linkline form_spatial_line';
    }
    else
    {
        new_link_text = "<i>Empty Spatial Coverage</i>";
        linkspan.className = 'form_linkline_empty form_spatial_line';
    }
            
    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( windowname );    
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}


function close_zootaxon_form(windowname) {
    diff = windowname.replace('form_zootaxon_', '');
    var linkspan = document.getElementById('form_zootaxon_term_' + diff);

    new_link_text = '';

    var kingdominput = document.getElementById('formzootaxonkingdom_' + diff);
    if ((kingdominput != null) && ((trimString(kingdominput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(kingdominput.value);

    var phyluminput = document.getElementById('formzootaxonphylum_' + diff);
    if ((phyluminput != null) && ((trimString(phyluminput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(phyluminput.value);

    var classinput = document.getElementById('formzootaxonclass_' + diff);
    if ((classinput != null) && ((trimString(classinput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(classinput.value);

    var orderinput = document.getElementById('formzootaxonorder_' + diff);
    if ((orderinput != null) && ((trimString(orderinput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(orderinput.value);

    var familyinput = document.getElementById('formzootaxonfamily_' + diff);
    if ((familyinput != null) && ((trimString(familyinput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(familyinput.value);

    var genusinput = document.getElementById('formzootaxongenus_' + diff);
    if ((genusinput != null) && ((trimString(genusinput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(genusinput.value);

    var speciesinput = document.getElementById('formzootaxonspecies_' + diff);
    if ((speciesinput != null) && ((trimString(speciesinput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(speciesinput.value);

    var commoninput = document.getElementById('formzootaxoncommon_' + diff);
    if ((commoninput != null) && ((trimString(commoninput.value)).length > 0))
        new_link_text = new_link_text + ' -- ' + trimString(commoninput.value);
  

    if (new_link_text.length > 0) {
        new_link_text = new_link_text.substring(4);
        linkspan.className = 'form_linkline form_zootaxon_line';
    }
    else {
        new_link_text = "<i>No taxonomic information</i>";
        linkspan.className = 'form_linkline_empty form_taxon_line';
    }

    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown(windowname);

    // Return focus
    linkspan.focus();

    // Return false to prevent a return trip to the server
    return false;
}

function close_maintitle_form( windowname )
{
    var linkspan = document.getElementById('form_maintitle_term');
    
    new_link_text = '';
        
    var nonsort_input = document.getElementById('formmaintitlenonsort' );
    if (( nonsort_input != null ) && ( (trimString(nonsort_input.value)).length > 0 ))
        new_link_text = new_link_text + nonsort_input.value;
        
    var title_input = document.getElementById('formmaintitletitle' );
    if (( title_input != null ) && ( (trimString(title_input.value)).length > 0 ))
        new_link_text = new_link_text + trimString(title_input.value);
        
    var subtitle_input = document.getElementById('formmaintitlesubtitle' );
    if (( subtitle_input != null ) && ( (trimString(subtitle_input.value)).length > 0 ))
        new_link_text = new_link_text + ' : ' + trimString(subtitle_input.value);
        
    if ( new_link_text.length == 0 )
    {
        new_link_text = "<i>Empty Main Title</i>";
        linkspan.className='form_linkline_empty form_title_main_line';
    }
    else
    {
        linkspan.className='form_linkline form_title_main_line';
    }
           
    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( windowname );    
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function close_othertitle_form( windowname )
{
    diff = windowname.replace('form_othertitle_','');
    var linkspan = document.getElementById('form_othertitle_line_' + diff);
    
    new_link_text = '';
        
    var nonsort_input = document.getElementById('formothertitlenonsort_' + diff );
    if (( nonsort_input != null ) && ( (trimString(nonsort_input.value)).length > 0 ))
        new_link_text = new_link_text + nonsort_input.value;
        
    var title_input = document.getElementById('formothertitletitle_' + diff );
    if (( title_input != null ) && ( (trimString(title_input.value)).length > 0 ))
        new_link_text = new_link_text + trimString(title_input.value);
        
    if ( new_link_text.length > 0 )
    {        
        var subtitle_input = document.getElementById('formothertitlesubtitle_' + diff );
        if (( subtitle_input != null ) && ( (trimString(subtitle_input.value)).length > 0 ))
            new_link_text = new_link_text + ' : ' + trimString(subtitle_input.value);
            
        var type_input = document.getElementById('formothertitletype_' + diff );
        switch ( type_input.value )
        {
            case "abbreviated":
                new_link_text = new_link_text + " ( <i>Abbreviated Title</i> )";
                break;
                
            case "uniform":
                new_link_text = new_link_text + " ( <i>Uniform Title</i> )";
                break;
                
            case "series":
                new_link_text = new_link_text + " ( <i>Series Title</i> )";
                break;
                
            case "translated":
                new_link_text = new_link_text + " ( <i>Translated Title</i> )";
                break;
                
            default:
                new_link_text = new_link_text + " ( <i>Alternative Title</i> )";
                break;
        }   
        linkspan.className='form_linkline form_title_main_line'; 
    }
    else
    {
        new_link_text = '<i>Empty Other Title</i>';
        linkspan.className='form_linkline_empty form_title_main_line';
    }

    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( windowname );    
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function other_title_type_change( titlenumber )
{
    var type_select = document.getElementById("formothertitletype_" + titlenumber);
    var display_label_span = document.getElementById("formothertitlesubtype_" + titlenumber);
    var select_box = document.getElementById("formothertitledisplay_" + titlenumber);
    
    var selected_value = type_select.value;
    switch( selected_value )
    {
        case "alternate":
            select_box_clear_options(select_box);
            select_box_add_option(select_box, "Added title page title", "added");
            select_box_add_option(select_box, "Alternate title", "alternate");
            select_box_add_option(select_box, "Caption title", "caption");
            select_box_add_option(select_box, "Cover title", "cover");
            select_box_add_option(select_box, "Distinctive title", "distinctive");
            select_box_add_option(select_box, "Other title", "other");
            select_box_add_option(select_box, "Portion of title", "portion");
            select_box_add_option(select_box, "Parallel title", "parallel");
            select_box_add_option(select_box, "Running title", "running");
            select_box_add_option(select_box, "Spine title", "spine");
            display_label_span.style.display = 'inline';
            break;

        case "uniform":
            select_box_clear_options(select_box);
            select_box_add_option(select_box, "Uniform Title", "uniform");
            select_box_add_option(select_box, "Main Entry", "main");
            select_box_add_option(select_box, "Uncontrolled Added Entry", "uncontrolled");
            display_label_span.style.display = 'inline';
            break;
            
        default:
            display_label_span.style.display = 'none';
            break;
    }
}

function select_box_clear_options(selectbox)
{
    var options=selectbox.getElementsByTagName("option");   
    var i;
    for (i=options.length-1; i>=0; i--)   {      selectbox.removeChild(options[i]);   }
}

function select_box_add_option(selectbox, text, value) {
    var optn = document.createElement("OPTION");
    optn.text = text;
    optn.value = value;
    selectbox.options.add(optn);
}


function close_ead_form()
{
    var linkspan = document.getElementById('form_ead_term');
    
    new_link_text = '';
    
    var url_input = document.getElementById('formead_url' );
    if (( url_input != null ) && ( (trimString(url_input.value)).length > 0 ))
        new_link_text = trimString(url_input.value);
        
    var name_input = document.getElementById('formead_name' );
    if (( name_input != null ) && ( (trimString(name_input.value)).length > 0 ))
        new_link_text = trimString(name_input.value);
        
    if ( new_link_text.length == 0 )
    {
        new_link_text = '<i>Empty Related EAD </i>';
        linkspan.className='form_linkline_empty form_ead_line';
    }
    else
    {
        linkspan.className='form_linkline form_ead_line';
    }
        
    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( 'form_ead' );    
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function close_related_url_form()
{
    var linkspan = document.getElementById('form_related_url_term');
    
    new_link_text = '';
    
    // Look for an existing URL.. if so, include display label as well
    var url_input = document.getElementById('form_relatedurl_url' );
    if (( url_input != null ) && ( (trimString(url_input.value)).length > 0 ))
    {
        new_link_text = trimString(url_input.value);
        
        var name_input = document.getElementById('form_relatedurl_label' );
        if (( name_input != null ) && ( (trimString(name_input.value)).length > 0 ))
            new_link_text = new_link_text + ' ( <i>' + trimString(name_input.value) + '</i> )'; 
    }
    
    if ( new_link_text.length == 0)
    {
        new_link_text = '<i>Empty Related URL </i>';
        linkspan.className='form_linkline_empty form_related_url_line';
    }
    else
    {
        linkspan.className = 'form_linkline form_related_url_line';
    }
    
    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( 'form_related_url' );   
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function close_typeformat_form()
{
    var linkspan = document.getElementById('form_typeformat_term');
    
    new_link_text = '';
    
    var extent_input = document.getElementById('form_typeformat_extent' );
    if (( extent_input != null ) && ( (trimString(extent_input.value)).length > 0 ))
        new_link_text = trimString(extent_input.value) + ' -- '; 
        
    var placecode_input = document.getElementById('form_typeformat_placecode' );
    if (( placecode_input != null ) && ( (trimString(placecode_input.value)).length > 0 ))
        new_link_text = new_link_text + trimString(placecode_input.value) + ' -- '; 
            
    var langcode_input = document.getElementById('form_typeformat_langcode' );
    if (( langcode_input != null ) && ( (trimString(langcode_input.value)).length > 0 ))
        new_link_text = new_link_text + trimString(langcode_input.value) + ' -- '; 
        
    if ( new_link_text.length < 4 )
    {
        new_link_text = '<i>no additional type information<i>';
        linkspan.className='form_linkline_empty form_typeformat_line';
    }
    else
    {
        new_link_text = new_link_text.substring( 0, new_link_text.length - 4 );
        linkspan.className = 'form_linkline form_typeformat_line';
    }
    
    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( 'form_typeformat' );   
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function close_related_item_form( windowname )
{
    diff = windowname.replace('form_related_item_','');
    var linkspan = document.getElementById('form_related_item_term_' + diff);
    
    new_link_text = '';
        
    var title_input = document.getElementById('form_relateditem_title_' + diff );
    if (( title_input != null ) && ( (trimString(title_input.value)).length > 0 ))
    {
        // Is there a display label?
        var display_input = document.getElementById('form_relateditem_display_' + diff );
        if (( display_input != null ) && ( (trimString(display_input.value)).length > 0 ))
        {    
            new_link_text = '( <i>' + display_input.value + '</i> ) ';
        }
        else
        {
            var relation_input = document.getElementById('form_relateditem_relation_' + diff );
            if (( relation_input != null ) && ( (trimString(relation_input.value)).length > 0 ))
            {
                switch( relation_input.value )
                {
                    case 'host':
                        new_link_text = '( <i>Host</i> ) ';
                        break;
                        
                    case 'other_format':
                        new_link_text = '( <i>Other Format</i> ) ';
                        break;   
                        
                    case 'other_version':
                        new_link_text = '( <i>Other Version</i> ) ';
                        break; 
                        
                    case 'preceding':
                        new_link_text = '( <i>Preceding</i> ) ';
                        break;   
                        
                    case 'succeeding':
                        new_link_text = '( <i>Succeeding</i> ) ';
                        break; 
                }
            }        
        }
        
        // Finally, add the title to the end
        new_link_text = new_link_text + title_input.value;
        
        linkspan.className = 'form_linkline form_related_item_line';
    }
    else
    {
        new_link_text = '<i>Empty Related Item</i>';
        linkspan.className = 'form_linkline_empty form_related_item_line';
    }
        


    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( windowname );   
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}



function close_serial_hierarchy()
{
    var linkspan = document.getElementById('form_serial_hierarchy_term');
    
    new_link_text = '';
    
    // first determine whether to use enumeration or chronology
    var type_input = document.getElementById('form_serialhierarchy_primary' );
    if (( type_input != null ) && ( (trimString(type_input.value)).length > 0 ))
    {
        serial_type = 'enum';
        if ( !type_input.checked )
            serial_type = 'chrono';
        
        if ( serial_type == 'enum' )
        {
            var enum1text_input = document.getElementById('form_serialhierarchy_enum1text' );
            var enum1order_input = document.getElementById('form_serialhierarchy_enum1order' );
            if (( enum1text_input != null ) && ( enum1order_input != null ) && (trimString(enum1text_input.value).length > 0 ))
                new_link_text = trimString(enum1text_input.value) + ' (' + trimString(enum1order_input.value) + ')';
                
            var enum2text_input = document.getElementById('form_serialhierarchy_enum2text' );
            var enum2order_input = document.getElementById('form_serialhierarchy_enum2order' );
            if (( enum2text_input != null ) && ( enum2order_input != null ) && (trimString(enum2text_input.value).length > 0 ))
            {
                if ( new_link_text.length > 0 )
                {
                    new_link_text = new_link_text + ' -- ' + (enum2text_input.value) + ' (' + trimString(enum2order_input.value) + ')';
                }
                else
                {
                    new_link_text = trimString(enum2text_input.value) + ' (' + trimString(enum2order_input.value) + ')';
                }
            }

            var enum3text_input = document.getElementById('form_serialhierarchy_enum3text' );
            var enum3order_input = document.getElementById('form_serialhierarchy_enum3order' );
            if (( enum3text_input != null ) && ( enum3order_input != null ) && (trimString(enum3text_input.value).length > 0 ))
            {
                if ( new_link_text.length > 0 )
                {
                    new_link_text = new_link_text + ' -- ' + (enum3text_input.value) + ' (' + trimString(enum3order_input.value) + ')';
                }
                else
                {
                    new_link_text = trimString(enum3text_input.value) + ' (' + trimString(enum3order_input.value) + ')';
                }
            }       
        }
        else
        {
            var chrono1text_input = document.getElementById('form_serialhierarchy_chrono1text' );
            var chrono1order_input = document.getElementById('form_serialhierarchy_chrono1order' );
            if (( chrono1text_input != null ) && ( chrono1order_input != null ) && (trimString(chrono1text_input.value).length > 0 ))
                new_link_text = trimString(chrono1text_input.value) + ' (' + trimString(chrono1order_input.value) + ')';
                
            var chrono2text_input = document.getElementById('form_serialhierarchy_chrono2text' );
            var chrono2order_input = document.getElementById('form_serialhierarchy_chrono2order' );
            if (( chrono2text_input != null ) && ( chrono2order_input != null ) && (trimString(chrono2text_input.value).length > 0 ))
            {
                if ( new_link_text.length > 0 )
                {
                    new_link_text = new_link_text + ' -- ' + (chrono2text_input.value) + ' (' + trimString(chrono2order_input.value) + ')';
                }
                else
                {
                    new_link_text = trimString(chrono2text_input.value) + ' (' + trimString(chrono2order_input.value) + ')';
                }
            }

            var chrono3text_input = document.getElementById('form_serialhierarchy_chrono3text' );
            var chrono3order_input = document.getElementById('form_serialhierarchy_chrono3order' );
            if (( chrono3text_input != null ) && ( chrono3order_input != null ) && (trimString(chrono3text_input.value).length > 0 ))
            {
                if ( new_link_text.length > 0 )
                {
                    new_link_text = new_link_text + ' -- ' + (chrono3text_input.value) + ' (' + trimString(chrono3order_input.value) + ')';
                }
                else
                {
                    new_link_text = trimString(chrono3text_input.value) + ' (' + trimString(chrono3order_input.value) + ')';
                }
            } 
        }        
    }    
    
    
    if ( new_link_text.length == 0)
    {
        new_link_text = '<i>Empty Serial Hierarchy </i>';
        linkspan.className='form_linkline_empty form_serial_hierarchy_line';
    }
    else
    {
        linkspan.className = 'form_linkline form_serial_hierarchy_line';
    }
    

    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( 'form_serial_hierarchy' );   
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function name_type_changed( nameid )
{
    // Get this select box
    var name_type_input = document.getElementById('form_name_type_' + nameid );
    if ( name_type_input != null )
    {
        // If this is personal, set the label correctly and hide some rows
        var desc_location_span = document.getElementById('name_desc_location_span_' + nameid);   
        var personal_label_1 = document.getElementById('name_personallabel1_' + nameid );
        var personal_label_2 = document.getElementById('name_personallabel2_' + nameid );
        var personal_label_3 = document.getElementById('name_personallabel3_' + nameid );
        var personal_label_4 = document.getElementById('name_personallabel4_' + nameid );
        var personal_input_1 = document.getElementById('form_name_given_' + nameid );
        var personal_input_2 = document.getElementById('form_name_family_' + nameid );
        var personal_input_3 = document.getElementById('form_name_display_' + nameid );
        var personal_input_4 = document.getElementById('form_name_terms_' + nameid );
        var form_obj = document.getElementById('form_name_' + nameid );
        
        if ( name_type_input.value == 'personal' )
        {
            desc_location_span.innerHTML = 'Description';
            personal_label_1.style.display = 'inline';
            personal_label_2.style.display = 'inline';
            personal_label_3.style.display = 'inline';
            personal_label_4.style.display = 'inline';
            personal_input_1.style.display = 'inline';
            personal_input_2.style.display = 'inline';
            personal_input_3.style.display = 'inline';
            personal_input_4.style.display = 'inline';
            form_obj.className = 'name_popup_div_personal sbkMetadata_PopupDiv';
        }
        else
        {
            desc_location_span.innerHTML = 'Location';
            personal_label_1.style.display = 'none';
            personal_label_2.style.display = 'none';
            personal_label_3.style.display = 'none';
            personal_label_4.style.display = 'none';
            personal_input_1.style.display = 'none';
            personal_input_2.style.display = 'none';
            personal_input_3.style.display = 'none';
            personal_input_4.style.display = 'none';
            form_obj.className = 'name_popup_div sbkMetadata_PopupDiv';
        }
    }
}


function close_name_form( windowname )
{
    diff = windowname.replace('form_name_','');
    var linkspan = document.getElementById('form_name_line_' + diff);    
    new_link_text = '';
    
    // Declare the empty strings for the following logic
    full_name = '';
    given_name = '';
    family_name = '';
    type = 'personal';
    
    // Get the values for the above strings
    var type_input = document.getElementById('form_name_type_' + diff );
    if (( type_input != null ) && ( (trimString(type_input.value)).length > 0 ))
        type = type_input.value;
        
    var full_input = document.getElementById('form_name_full_' + diff );
    if (( full_input != null ) && ( (trimString(full_input.value)).length > 0 ))
        full_name = full_input.value;
        
    // Look for given and family name in the case this is a personal name and full is empty
    if (( type == 'personal' ) && ( full_name.length == 0 ))
    {        
        var given_input = document.getElementById('form_name_given_' + diff );
        if (( given_input != null ) && ( (trimString(given_input.value)).length > 0 ))
            given_name = trimString(given_input.value);
            
        var family_input = document.getElementById('form_name_family_' + diff );
        if (( family_input != null ) && ( (trimString(family_input.value)).length > 0 ))
            family_name = trimString(family_input.value);
            
        if ( given_name.length > 0 )
        {
            if ( family_name.length > 0 )
            {
                new_link_text = family_name + ", " + given_name;
            }
            else
            {
                new_link_text = given_name;
            }        
        }
        else
        {
            new_link_text = family_name;        
        }
        
        var display_form = document.getElementById('form_name_display_' + diff );
        if (( display_form != null ) && ( (trimString(display_form.value)).length > 0 ))
            new_link_text = new_link_text + ' ( ' + trimString(display_form.value) + ' )';
            
        var dates = document.getElementById('form_name_dates_' + diff );
        if (( dates != null ) && ( (trimString(dates.value)).length > 0 ))
            new_link_text = new_link_text + ', ' + trimString(dates.value);    
    }
    else
    {
        new_link_text = full_name;
    }       
        
    if ( new_link_text.length > 0 )
    {
        // Collect the roles
        roles = '';
        var role1_input = document.getElementById('form_name_role1_' + diff );
        if (( role1_input != null ) && ( (trimString(role1_input.value)).length > 0 ))
        {
            roles = (trimString(role1_input.value));
        }
               
        var role2_input = document.getElementById('form_name_role2_' + diff );
        if (( role2_input != null ) && ( (trimString(role2_input.value)).length > 0 ))
        {
            if ( roles.length > 0 )
            {
                roles = roles + ', ' + trimString(role2_input.value);
            }
            else
            {
                roles = trimString(role2_input.value);
            }    
        }
                   
        var role3_input = document.getElementById('form_name_role3_' + diff );
        if (( role3_input != null ) && ( (trimString(role3_input.value)).length > 0 ))
        {
            if ( roles.length > 0 )
            {
                roles = roles + ', ' + trimString(role3_input.value);
            }
            else
            {
                roles = trimString(role3_input.value);
            }    
        } 
        
        // Add the roles, if there were some
        if ( roles.length > 0 )
            new_link_text = new_link_text + ' ( <i>' + roles + '</i> )';
    
        linkspan.className = 'form_linkline form_name_line';
    }
    else
    {
        new_link_text = "<i>Empty Name</i>";
        linkspan.className = 'form_linkline_empty form_name_line';
    }
        
    // Use the new calculated text for the link    
    linkspan.innerHTML = new_link_text;

    // Close the associated form
    popdown( windowname );    
    
    // Return focus
    linkspan.focus();
    
    // Return false to prevent a return trip to the server
    return false;
}

function set_uf_rights(textarea_name, new_value) {
    var textarea = document.getElementById(textarea_name);
    textarea.value = new_value;

    var rights_table = document.getElementById('uf_rights');
    if (rights_table != null)
        toggle('uf_rights');

    return false;

}

function set_cc_rights( textarea_name, new_value )
{
    var textarea = document.getElementById(textarea_name);  
    textarea.value = new_value;
    
    var rights_table = document.getElementById('cc_rights');
	if (rights_table != null)
		toggle('cc_rights');
        
    return false;
}

function open_cc_rights()
{
    //First ensure the uf_rights table is hidden
    var uf_rights = document.getElementById('uf_rights');
    uf_rights.style.display='none';

    toggle('cc_rights');	
    return false;
}

function open_uf_rights() {
    //First ensure the cc_rights table is hidden
    var cc_rights = document.getElementById('cc_rights');
    cc_rights.style.display = 'none';

    toggle('uf_rights');
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
    if ( linkspan.className.indexOf('_focused') > 0 )
        linkspan.className = linkspan.className.replace( '_focused', '')
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

function new_creator_link_clicked( page )
{
    // Set the hidden value based on the user request
    var hiddenfield = document.getElementById('new_element_requested');
    hiddenfield.value = 'name';
    
	// Perform post back
    document.itemNavForm.submit();
    return false;
}

function new_title_link_clicked( page )
{
    // Set the hidden value based on the user request
    var hiddenfield = document.getElementById('new_element_requested');
    hiddenfield.value = 'title';
    
	// Perform post back
    document.itemNavForm.submit();
    return false;
}

function new_subject_link_clicked( page )
{
    // Set the hidden value based on the user request
    var hiddenfield = document.getElementById('new_element_requested');
    hiddenfield.value = 'subject';
    
	// Perform post back
    document.itemNavForm.submit();
    return false;
}

function new_spatial_link_clicked( page )
{
    // Set the hidden value based on the user request
    var hiddenfield = document.getElementById('new_element_requested');
    hiddenfield.value = 'spatial';
    
	// Perform post back
    document.itemNavForm.submit();
    return false;
}

function new_relateditem_link_clicked( page )
{
    // Set the hidden value based on the user request
    var hiddenfield = document.getElementById('new_element_requested');
    hiddenfield.value = 'relateditem';
    
	// Perform post back
    document.itemNavForm.submit();
    return false;
}

function editmetadata_cancel_form()
{
    // Set the hidden value based on the user request
    var hiddenfield = document.getElementById('new_element_requested');
    hiddenfield.value = 'cancel';
    
	// Perform post back
    document.itemNavForm.submit();
    return false;
}

function editmetadata_save_form()
{
    // Set the hidden value based on the user request
    var hiddenfield = document.getElementById('new_element_requested');
    hiddenfield.value = 'save';
    
	// Perform post back
    document.itemNavForm.submit();
    return false;
}

function editmetadata_newpage(Page) {
	// Set the hidden value based on the user request
	var hiddenfield = document.getElementById('new_element_requested');
	hiddenfield.value = 'newpage' + Page;

	// Perform post back
	document.itemNavForm.submit();
	return false;
}

function editmetadata_complicate() {
	// Set the hidden value based on the user request
	var hiddenfield = document.getElementById('new_element_requested');
	hiddenfield.value = 'complicate';

	// Perform post back
	document.itemNavForm.submit();
	return false;
}

function editmetadata_simplify() {
	// Set the hidden value based on the user request
	var hiddenfield = document.getElementById('new_element_requested');
	hiddenfield.value = 'simplify';

	// Perform post back
	document.itemNavForm.submit();
	return false;
}


// Trim the input string from the search box
function trimString (str) 
{
  str = this != window? this : str;
  return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}


//Multi-file upload - autonumber the label fields
function upload_label_fieldChanged(TextboxID, TotalFileCount) {

 //   alert(TotalFileCount);
         var textboxValue = document.getElementById(TextboxID).value;

  
        //if only a number was entered (e.g. '5'), add text 'Page ' (i.e. 'Page 5') 
        var onlyNumberEntered = ((!isNaN(parseFloat(textboxValue))) && isFinite(textboxValue));
        if (onlyNumberEntered) {
            document.getElementById(TextboxID).value = 'Page ' + textboxValue;
            textboxValue = document.getElementById(TextboxID).value;
        }

        var index = TextboxID.replace('upload_label', '');
        var lastNumber = textboxValue.split(" ")[(textboxValue.split(" ").length - 1)];

        //	lastNumber = lastNumber.toUpperCase().trim();
        var matches = lastNumber.match(/\d+/g);
        var varRomanMatches = true;
        var isRomanLower = true;

        // Look for any potential roman numeral matches at the end of the string
        for (var x = 0; x < lastNumber.length; x++) {
            var c = lastNumber.charAt(x);
            if ("IVXLCDM".indexOf(c) == -1 && "ivxlcdm".indexOf(c) == -1) {
                varRomanMatches = false;
            }
        }

        // Was there a match for numbers in the last portion of the textbox value?
        if (matches != null) {
            //if the number is at the end of the string, with a space before
            if (matches[0].length == lastNumber.length) {

                var textOnlyLastBox = textboxValue.substr(0, textboxValue.length - matches[0].length);


                var number = parseInt(lastNumber);
               
                 for (var i = parseInt(index) + 1; i <= TotalFileCount; i++) {
      
                    // Determine and save the next numeric value
                  if (document.getElementById('upload_label' + i)) {
                        number++;
                        var inLoopTextBoxElment = document.getElementById('upload_label' + i);
                        inLoopTextBoxElment.value = textOnlyLastBox + number;
                    }
                    else
                        continue;
                }

                //var hidden_filename = document.getElementById(spanArray[spanArray.length - 1].replace('span', 'filename'));
                //document.getElementById('Autonumber_last_filename').value = hidden_filename.value;
            }//end if
        }//end if
        else if (varRomanMatches == true) {

            var romanToNumberError = "No error";

          //Determine whether the roman number is in upper or lower case
            for (var x = 0; x < lastNumber.length; x++) {
                var c = lastNumber.charAt(x);
                if ("IVXLCDM".indexOf(c) > -1) {
                    isRomanLower = false;
                }
                else {
                    isRomanLower = true;
                }
            }



            var roman = lastNumber.toUpperCase().trim();

            if (roman.split('V').length > 2 || roman.split('L').length > 2 || roman.split('D').length > 2) {
                romanToNumberError = "Repeated use of V,L or D";
            }
            //Check that a single letter is not repeated more than thrice
            var count = 1;
            var last = 'Z';
            for (var x = 0; x < roman.length; x++) {
                //Duplicate?
                if (roman.charAt(x) == last) {
                    count++;
                    if (count == 4) {
                        romanToNumberError = "Single letter repeated more than thrice";
                    }

                }
                else {
                    count = 1;
                    last = roman.charAt(x);
                }
            }

            //Create an arraylist containing the values
            var ptr = 0;
            var values = new Array();
            var maxDigit = 1000;
            var digit = 0;
            var nextDigit = 0;

            while (ptr < roman.length) {
                //Base value of digit
                var numeral = roman.charAt(ptr);
                switch (numeral) {
                    case "I":
                        digit = 1;
                        break;
                    case "V":
                        digit = 5;
                        break;
                    case "X":
                        digit = 10;
                        break;
                    case "L":
                        digit = 50;
                        break;
                    case "C":
                        digit = 100;
                        break;
                    case "D":
                        digit = 500;
                        break;
                    case "M":
                        digit = 1000;
                        break;

                }
                //Check for subtractive combination: A small valued numeral may be placed to the left of a larger value. When this occurs, the smaller numeral is subtracted
                //from the larger. Also, the subtracted digit must be at least 1/10th the value of the larger numeral and must be either I,X or C
                if (digit > maxDigit) {
                    romanToNumberError = "Smaller value incorrectly placed to the left of a larger value numeral";
                }

                //Next digit
                var nextDigit = 0;
                if (ptr < roman.length - 1) {
                    var nextNumeral = roman.charAt(ptr + 1);
                    switch (nextNumeral) {
                        case "I":
                            nextDigit = 1;
                            break;
                        case "V":
                            nextDigit = 5;
                            break;
                        case "X":
                            nextDigit = 10;
                            break;
                        case "L":
                            nextDigit = 50;
                            break;
                        case "C":
                            nextDigit = 100;
                            break;
                        case "D":
                            nextDigit = 500;
                            break;
                        case "M":
                            nextDigit = 1000;
                            break;

                    }
                    if (nextDigit > digit) {
                        if ("IXC".indexOf(numeral) == -1 || nextDigit > (digit * 10) || roman.split(numeral).length > 3) {
                            romanToNumberError = "Rule of subtractive combination violated";
                        }
                        maxDigit = digit - 1;
                        digit = nextDigit - digit;
                        ptr++;
                    }

                }
                values.push(digit);
                //next digit
                ptr++;


            }
            //Going left to right - the value should not increase
            for (var i = 0; i < values.length - 1; i++) {
                if (values[i] < values[i + 1]) {
                    romanToNumberError = "Larger valued numeral(pair) found to the right of a smaller value";
                }
            }

            //Larger numerals should be placed to the left of smaller numerals
            var total = 0;
            for (var i = 0; i < values.length; i++) {
                total = total + values[i];
            }

            if ((typeof total) == "number" && (romanToNumberError == "No error")) {

                //If only a roman numeral was entered, add the text 'Page' before the numeral
                if (lastNumber == document.getElementById(TextboxID).value) {
                    document.getElementById(TextboxID).value = 'Page ' + textboxValue;
                    textboxValue = document.getElementById(TextboxID).value;
                }

                //Now autonumber all the remaining textboxes of the document
                for (var i = parseInt(index) + 1; i <= TotalFileCount ; i++) {
                    if (document.getElementById('upload_label' + i)) {
                        total++;
                    
                        var number = total;

                        //Convert decimal "total" back to a roman numeral

                        //Set up the key-value arrays
                        var values = [1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1];
                        var numerals = ["M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"];

                        //initialize the string
                        var result = "";

                        for (var x = 0; x < 13; x++) {
                            //If the number being converted is less than the current key value, append the corresponding numeral or numerical pair to the resultant string
                            while (number >= values[x]) {
                                number = number - values[x];
                                result = result + numerals[x];
                             
                            }
                            
                        }
                 

                        if (isRomanLower) {
                            result = result.toLowerCase();
           
                        }
                      
                        //End conversion to roman numeral

                        document.getElementById('upload_label' + i).value =
                            document.getElementById(TextboxID).value.substr(0, ((document.getElementById(TextboxID).value.length) - (lastNumber.length)) - 1) + ' ' + result;

                        //           textOnlyLastBox.value = document.getElementById(TextboxID).value.substr(0, ((document.getElementById(TextboxID).value.length) - (lastNumber.length)) - 1) + ' ';
                        //          numberOnlyLastBox.value = total;
                    }
                }//end for loop
            }
        }


}