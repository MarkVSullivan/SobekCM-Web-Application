var spanArrayObjects;
var spanArray;
var autonumberingMode=-1;
var makeSortable = 3; //indicates sorting mode
var cursorMode = 1;
var lastSelected = -1;
var thumbnailImageDictionary = {};
var qc_image_folder;
var page_index_popup='';

// Function to set the full screen mode 
function qc_set_fullscreen() {
    // set the height (starting under the header, etc..)
    var y = $("#allThumbnailsOuterDiv1").offset().top;
    var window_height = $(window).height();
    var new_height = Math.floor(window_height - y - 63);
    $("#allThumbnailsOuterDiv1").height(new_height);
    
    // set the width
    var window_width = Math.floor($(window).width()) - 12;
    $("#allThumbnailsOuterDiv1").width(window_width);
    
    // Save these values
    document.getElementById("QC_window_height").value = new_height.toString();
    document.getElementById("QC_window_width").value = window_width.toString();
	
	//Adjust the menu "Go to thumbnail:" feature based on the window width
	if(window_width>1022)
	  {
	    document.getElementById('GoToThumbnailTextSpan').innerHTML='GO TO THUMBNAIL:';
		document.getElementById('sbkQc_GoToThumbnailDiv').style.visibility = 'visible';
	  }
	else if(window_width<1022 && window_width>999)
	 {
     	document.getElementById('GoToThumbnailTextSpan').innerHTML='GO TO:';
	    document.getElementById('sbkQc_GoToThumbnailDiv').style.visibility = 'visible';
	 }
	
	else if(window_width<999)
	{
      document.getElementById('GoToThumbnailTextSpan').innerHTML='GO TO THUMBNAIL:';
	  document.getElementById('sbkQc_GoToThumbnailDiv').style.visibility='hidden';
	}
	  
}


function Save_Image_Folder(location) {
    qc_image_folder = location+"qc/";
}


//Assign the thumbnail image filenames, file locations to the global dictionary
function QC_Add_Image_To_Dictionary(filename, file_location) {
 //   alert(filename+' '+file_location);
    thumbnailImageDictionary[filename] = file_location;

   // alert(qc_image_folder);
    //   alert('thumbnailImageDictionary[' + filename + ']=' + thumbnailImageDictionary[filename]);

}



//Called when the user clicks on one of the sorting options in the menu 
function QC_Change_Sortable_Setting(option, image_location)
{
  //Assign the sorting option to the global variable. 
    makeSortable = option;
  
  //Set the hidden variable value
  var hidden_sortable_option = document.getElementById('QC_sortable_option'); 
  hidden_sortable_option.value = makeSortable;

  
  //First uncheck both the options
  var enableID = document.getElementById('checkmarkEnableSorting');
  var disableID = document.getElementById('checkmarkDisableSorting');
  var enableWithConfID = document.getElementById('checkmarkEnableSorting_conf');
  
  enableID.src=image_location+"noCheckmark.png";
  disableID.src = image_location+"noCheckmark.png";
  enableWithConfID.src = image_location+"noCheckmark.png";
  
  //Now check the selected option
  if(option==1)
   {
     enableID.src=image_location+"checkmark.png";
	  $("#allThumbnailsOuterDiv").sortable("enable");
   }
  else if(option==2)
  {
    enableWithConfID.src = image_location+"checkmark.png";
	$("#allThumbnailsOuterDiv").sortable("enable");
  }
  else
  {
    disableID.src = image_location+"checkmark.png";
    $("#allThumbnailsOuterDiv").sortable("disable");
  }
    // Close the superfish menu
   $('ul.qc-menu').hideSuperfishUl();
  
}


//Check/uncheck the corresponding autonumbering option when selected in the menu
function Autonumbering_mode_changed(mode, image_location)
{
   //Set the global mode variable to this value
    autonumberingMode = mode;
    document.getElementById("QC_autonumber_option").value = mode;

//  alert('mode:'+mode+ 'image location'+image_location);
  
  //First uncheck all the autonumbering options in the menu
   for(var i=0;i<=2;i++)
   {

	   var uncheckedImageID = document.getElementById('checkmarkMode'+i);
	   uncheckedImageID.src=image_location+"noCheckmark.png";
	 
   }
  
   //Now check the selected autonumber mode option
   var imageID = document.getElementById('checkmarkMode'+mode);
   imageID.src= image_location+"checkmark.png";
  
    // Close the superfish menu
   $('ul.qc-menu').hideSuperfishUl();
}


function Configure_QC( MaxPageCount ) {
    //get the list of all the thumbnail spans on the page
    spanArrayObjects = $("#allThumbnailsOuterDiv").children();

    spanArray = new Array();

    //get the spanIDs from the array of span objects 
    var j = 0;
    for (var i = 0; i < spanArrayObjects.length; i++)
    {
        if (spanArrayObjects[i].id.indexOf('span') == 0)
        {
            spanArray[j++] = spanArrayObjects[i].id;
        }
    }
    
    autonumberingMode = document.getElementById("QC_autonumber_option").value;
	makeSortable = document.getElementById("QC_sortable_option").value;

}


//Updates the division types when a "New Division" checkbox is checked/unchecked
function UpdateDivDropdown(CheckBoxID) {
    var index = CheckBoxID.replace("newDivType", '');
    var checkBoxElement = document.getElementById(CheckBoxID);
    var selectDivTypeElement = document.getElementById('selectDivType' + index);
    var textDivNameElement = document.getElementById('txtDivName' + index);
    
    // Check the state of the checkbox 
    if (checkBoxElement.checked == true) {
        // This is the start of a new division, so enable the div-type controls
        selectDivTypeElement.disabled = false;
        textDivNameElement.disabled = false;
    } else {
        // This is no longer the beginning of a new division 
        selectDivTypeElement.disabled = true;

        // Enable the text div element, depending on if the curernt div can be named
        if (selectDivTypeElement.value.indexOf('!') == 0)
            textDivNameElement.disabled = false;
        else
            textDivNameElement.disabled = true;

        //update the subsequent divs
        var i = spanArray.indexOf('span' + index);

        // Since this is the first span on the page, set to MAIN.
        // Really, this needs to look up a hidden value in case this was not the first
        // page of thumbnails
        var divTypeToSet = 'Main';
        var divNameToSet = '';

        if (i > 0) {
            // Get the divtype from the previous division
            var previousSelectDivId = spanArray[i - 1].replace('span', 'selectDivType');
            divTypeToSet = document.getElementById(previousSelectDivId).value;
            var previousDivNameId = spanArray[i - 1].replace('span', 'txtDivName');
            divNameToSet = document.getElementById(previousDivNameId).value;
        }


        //set the page division type of all pages till the start of the next div
        while ((i < spanArray.length) && (document.getElementById(spanArray[i].replace('span', 'selectDivType')).disabled == true))
        {
            document.getElementById(spanArray[i].replace('span', 'selectDivType')).value = divTypeToSet;
            document.getElementById(spanArray[i].replace('span', 'txtDivName')).value = divNameToSet;
            if (divNameToSet.length > 0) {
                document.getElementById(spanArray[i].replace('span', 'divNameTableRow')).style.visibility='visible';
            } else {
                document.getElementById(spanArray[i].replace('span', 'divNameTableRow')).style.visibility = 'hidden';
            }
            i++;
        }
    }

    return true;
}

//Change all subsequent division types when one div type is changed. Also update the named division type textboxes as appropriate
function DivisionTypeChanged(SelectID)
{
    var index = SelectID.replace('selectDivType', '');
    var divisionTypeElement = document.getElementById(SelectID);

    var i = spanArray.indexOf('span' + index) + 1;
	var currVal = divisionTypeElement.value;

	//if the new Division type selected is a nameable div
	if (divisionTypeElement.value.indexOf('!') == 0)
	{
	    document.getElementById('divNameTableRow' + index).style.visibility = 'visible';
	    document.getElementById('divNameTableRow' + index).value = '';

	    //Make the name textboxes of all other pages of this div visible
	    while ((i < spanArray.length) && (document.getElementById(spanArray[i].replace('span','selectDivType')).disabled==true))
	    {
	        document.getElementById(spanArray[i].replace('span', 'selectDivType')).value = currVal;
	        document.getElementById(spanArray[i].replace('span','txtDivName')).disabled = true;
	        document.getElementById(spanArray[i].replace('span','divNameTableRow')).style.visibility='visible';
	        i++;
	    }
	}
	else if(currVal.indexOf('!')==-1)
	{
	    //else if the division type selected is not a nameable div
	    //Hide the name textbox for this page
	    document.getElementById('divNameTableRow' + index).style.visibility = 'hidden';
	
	    //Hide the name textboxes of all the other pages of this division type
	    while ((i < spanArray.length) && (document.getElementById(spanArray[i].replace('span', 'selectDivType')).disabled == true))
	    {
	        document.getElementById(spanArray[i].replace('span', 'selectDivType')).value = currVal;
	        document.getElementById(spanArray[i].replace('span', 'txtDivName')).value = '';
	        document.getElementById(spanArray[i].replace('span', 'divNameTableRow')).style.visibility = 'hidden';
	        i++;
	    }
	}
}


//Update the division name through the textboxes of the division type when changed in one
function DivNameTextChanged(TextboxID)
{
    var index = TextboxID.replace('txtDivName', '');
    var divisionNameElement = document.getElementById(TextboxID);
    var i = spanArray.indexOf('span' + index) + 1;
    var currVal = divisionNameElement.value;
  
    //Update the textboxes of the same division after this one
    while ((i < spanArray.length) && (document.getElementById(spanArray[i].replace('span', 'selectDivType')).disabled == true)) {
        document.getElementById(spanArray[i].replace('span', 'txtDivName')).value = currVal;
	    i++;
    }
}



//Autonumber subsequent textboxes on changing one textbox value
//parameter textboxID = the ID of the pagination textbox changed by the user
//parameter mode: the autonumbering mode passed in from the qc itemviewer. 0:auto number all the pages of the entire item, 1: all the pages of the current division only, 
//2: No auto numbering
//parameter MaxPageCount: the max number of pages for the current item in qc. This refers to all the pages, not just the ones displayed on the screen (though this makes no difference from the javascript point of view.)

function PaginationTextChanged(TextboxID)
{

    //Mode '0': Auto number all the thumbnail page names till the end
    //Mode '1': Auto number all the thumbnail pages till the start of the next div
    //Mode '2': No auto numbering
    
	//if there is a value assigned to the global autonumbering mode variable, use the global value insted of this one
//	if(autonumberingMode>-1)
//	  Mode = autonumberingMode;
    var Mode=autonumberingMode;
    if (Mode == 2)
        return;
    
//	alert('Mode:'+Mode);
	
    var textboxValue = document.getElementById(TextboxID).value;
	//if only a number was entered (e.g. '5'), add text 'Page ' (i.e. 'Page 5') 
    //var onlyNumberEntered = textboxValue.match(/\d+/g);
    var onlyNumberEntered = ((!isNaN(parseFloat(textboxValue))) && isFinite(textboxValue));
	if(onlyNumberEntered)
	  {
	     document.getElementById(TextboxID).value = 'Page '+ textboxValue;
		 textboxValue = document.getElementById(TextboxID).value;
	  }
    var index = TextboxID.replace('textbox', '');
	var lastNumber = textboxValue.split(" ")[(textboxValue.split(" ").length-1)];
	
	var textOnlyLastBox=document.getElementById('Autonumber_text_without_number');
	var numberOnlyLastBox=document.getElementById('Autonumber_number_only');
	
//	lastNumber = lastNumber.toUpperCase().trim();
	var matches = lastNumber.match(/\d+/g);
	var varRomanMatches = true;
	var isRomanLower=true;

    // Look for any potential roman numeral matches at the end of the string
	for(var x=0;x<lastNumber.length;x++)
	{
	    var c=lastNumber.charAt(x);
	    if("IVXLCDM".indexOf(c)==-1 && "ivxlcdm".indexOf(c)==-1)
	    {
		    varRomanMatches=false;
	    }
	}

	var hidden_filename = document.getElementById(spanArray[spanArray.length - 1].replace('span', 'filename'));
	document.getElementById('Autonumber_last_filename').value=hidden_filename.value;
	
    // Was there a match for numbers in the last portion of the textbox value?
	if (matches != null) 
	{
	   //if the number is at the end of the string, with a space before
	   if(matches[0].length == lastNumber.length)
	   {
	        //Set the QC form hidden variable with this mode
			document.getElementById('autonumber_mode_from_form').value = Mode;
	        document.getElementById('Autonumber_number_system').value = 'decimal';
	        textOnlyLastBox.value = textboxValue.substr(0, textboxValue.length - matches[0].length);
		
			
	        var number = parseInt(lastNumber);

			for(var i=spanArray.indexOf('span' + index)+1; i < spanArray.length;i++)
			{
			    // If this is MODE 1, then look to see if this is the beginnnig of a new division
			    if ((Mode == '1') && (document.getElementById(spanArray[i].replace('span', 'selectDivType')).disabled == false))
			        break;

                // Determine and save the next numeric value
			    number++;
			    numberOnlyLastBox.value = number.toString();

			    var inLoopTextBoxElment = document.getElementById(spanArray[i].replace('span', 'textbox'));
			    inLoopTextBoxElment.value = textOnlyLastBox.value + number;
			}

	        var hidden_filename = document.getElementById(spanArray[spanArray.length - 1].replace('span', 'filename'));
			document.getElementById('Autonumber_last_filename').value=hidden_filename.value;
	   }//end if
	}//end if
	else if(varRomanMatches==true)
	{

	   var romanToNumberError="No error";
	   
	   //Determine whether the roman number is in upper or lower case
	   for(var x=0;x<lastNumber.length;x++)
		{
		  var c=lastNumber.charAt(x);
		  if("IVXLCDM".indexOf(c)>-1)
		  {
			  isRomanLower=false;
		  }
		  else
		  {
			 isRomanLower =true;
		  }
		}
//	   alert(isRomanLower);
	   
   
		var roman = lastNumber.toUpperCase().trim();
		
		if(roman.split('V').length>2||roman.split('L').length>2||roman.split('D').length>2)
		{
			romanToNumberError="Repeated use of V,L or D";
		}	  
		//Check that a single letter is not repeated more than thrice
		var count=1;
		var last='Z';
		for(var x=0;x<roman.length;x++)
		{
		  //Duplicate?
		  if(roman.charAt(x)==last)
		  {
			count++;
			if(count==4)
			{
			  romanToNumberError="Single letter repeated more than thrice";
			}

		  }
		  else
		  {
			  count=1;
			  last = roman.charAt(x);
		  }
		}

		//Create an arraylist containing the values
		var ptr=0;
		var values = new Array();
		var maxDigit=1000;
		var digit=0;
		var nextDigit=0;
		
		while (ptr<roman.length)
		{
		  //Base value of digit
		  var numeral=roman.charAt(ptr);
		  switch(numeral)
		  {
		   case "I":
			 digit=1;
			 break;
		   case "V":
			 digit=5;
			 break;
		   case "X":
			  digit=10;
			  break;
		   case "L":
			digit=50;
			break;
		   case "C":
			 digit=100;
			 break;
		  case "D":
			  digit=500;
			  break;
		   case "M":
			  digit=1000;
			  break;
		  
		  }
		 //Check for subtractive combination: A small valued numeral may be placed to the left of a larger value. When this occurs, the smaller numeral is subtracted
		 //from the larger. Also, the subtracted digit must be at least 1/10th the value of the larger numeral and must be either I,X or C
		 if(digit>maxDigit)		 
		 {
		   romanToNumberError="Smaller value incorrectly placed to the left of a larger value numeral";
		 }
		 
		 //Next digit
		 var nextDigit=0;
		 if(ptr<roman.length-1)
		 {
		  var nextNumeral=roman.charAt(ptr+1);
		  switch(nextNumeral)
		  {
		   case "I":
			 nextDigit=1;
			 break;
		   case "V":
			 nextDigit=5;
			 break;
		   case "X":
			  nextDigit=10;
			  break;
		   case "L":
			nextDigit=50;
			break;
		   case "C":
			 nextDigit=100;
			 break;
		  case "D":
			  nextDigit=500;
			  break;
		   case "M":
			  nextDigit=1000;
			  break;
		  
		  }
		  if(nextDigit>digit)
		  {
			if("IXC".indexOf(numeral)==-1 || nextDigit>(digit*10) || roman.split(numeral).length>3)
			  {
			   romanToNumberError="Rule of subtractive combination violated";
			  }
			  maxDigit=digit-1;
			  digit=nextDigit-digit;
			  ptr++;
		  }
		  
		 }
		 values.push(digit);
		 //next digit
		 ptr++;
//		  alert(values);
		
		
		}
		//Going left to right - the value should not increase
		for(var i=0;i<values.length-1;i++)
		{
		  if(values[i]<values[i+1])
		  {
			romanToNumberError="Larger valued numeral(pair) found to the right of a smaller value";
		  }
		}
		
		//Larger numerals should be placed to the left of smaller numerals
		var total=0;
		for(var i=0;i<values.length;i++)
		{
		  total=total+values[i];
		}
		
		if((typeof total)=="number" && (romanToNumberError=="No error"))
		{

		   //Set the QC form hidden variable with this mode
		   var hidden_autonumber_mode = document.getElementById('autonumber_mode_from_form');
		   hidden_autonumber_mode.value = '0';
//		   alert('after setting the autonumber mode hidden variable');
		   
		   var hidden_number_system = document.getElementById('Autonumber_number_system');
		   hidden_number_system.value='ROMAN';

		 
		  //Now autonumber all the remaining textboxes of the document

  
		  for(var i=spanArray.indexOf('span'+TextboxID.split('textbox')[1])+1;i<=spanArray.length;i++)
			{
			  total++;

			  //If MODE 1: Check for the start of a new division
			  if ((Mode == '1') && (document.getElementById(spanArray[i].replace('span', 'selectDivType')).disabled == false))
			        break;
			  if(document.getElementById(spanArray[i].replace('span','textbox')))
			  {
			  
				var number=total;
				//alert('before beginning reconversion');
				//Convert decimal "total" back to a roman numeral
				
				//Set up the key-value arrays
				var values=[1000,900,500,400,100,90,50,40,10,9,5,4,1];
				var numerals=["M","CM","D","CD","C","XC","L","XL","X","IX","V","IV","I"];
				
				//initialize the string
				var result="";
				
				for(var x=0;x<13;x++)
				{
				  //If the number being converted is less than the current key value, append the corresponding numeral or numerical pair to the resultant string
				  while(number>=values[x])
				  {
					number=number-values[x];
					result=result+numerals[x];
					
				  }
				}
				
//				alert(result);
				if(isRomanLower)
				{
				  result=result.toLowerCase();
				  hidden_number_system.value='roman';
				}
				
				//End conversion to roman numeral

//				alert((document.getElementById(textboxID).value.length)-(lastNumber.length)-1);
				document.getElementById(spanArray[i].replace('span','textbox')).value = 
				 document.getElementById(TextboxID).value.substr(0,((document.getElementById(TextboxID).value.length)-(lastNumber.length))-1)+' '+result;
				 
		   textOnlyLastBox.value = document.getElementById(TextboxID).value.substr(0,((document.getElementById(TextboxID).value.length)-(lastNumber.length))-1)+' ';
		   numberOnlyLastBox.value = total;
			  }//end if(textbox found)
			}//end for loop
		}
	}
}//end function



//Show the QC Icons below the thumbnail on mouseover
function showQcPageIcons(SpanID)
{
    var qcPageIconsSpan = SpanID.replace('span', 'qcPageOptions');
    document.getElementById(qcPageIconsSpan).style.visibility = 'visible';
}

//Hide the QC Icon bar below the thumbnail on mouseout
function hideQcPageIcons(SpanID)
{
    var qcPageIconsSpan = SpanID.replace('span', 'qcPageOptions');
    document.getElementById(qcPageIconsSpan).style.visibility = 'hidden';
}

//Show the error icon on mouseover
function showErrorIcon(SpanID)
{
    var qcErrorIconSpanName = SpanID.replace('span', 'error');
    var qcErrorIconSpan = document.getElementById(qcErrorIconSpanName);
    if ( qcErrorIconSpan != null )
	    qcErrorIconSpan.className = "errorIconSpanHover";
}

//Hide the error icon on mouseout
function hideErrorIcon(SpanID)
{
    var qcErrorIconSpanName = SpanID.replace('span', 'error');
    var qcErrorIconSpan = document.getElementById(qcErrorIconSpanName);
    if (qcErrorIconSpan != null)
	    qcErrorIconSpan.className = "errorIconSpan";
}

//Change the cursor to the custom cursor for Selecting a Main Thumbnail
//On clicking on the "Pick Main Thumbnail" icon in the menu bar
function mainthumbnailicon_click()
{
	//If this cursor is already set, change back to default
	if(cursorMode == 2) {
	    ResetCursorToDefault();
	    $('#qc_mainmenu_default').addClass('sbkQc_MainMenuIconCurrent');
	}
	else
	{
		//Remove the default cursor style class, and any other custom class first before setting this one, 
		//otherwise it will override the custom cursor class
	    ResetCursorToDefault();
	    $('#qc_mainmenu_thumb').addClass('sbkQc_MainMenuIconCurrent');
	    
	    // Step through each span
	    for (var j = 0; j < spanArray.length; j++) {
	        // Get the span
	        var span = $('#' + spanArray[j]);
	        
	        span.removeClass('qcResetMouseCursorToDefault');
	        span.addClass('qcPickMainThumbnailCursor');
	    }
	    
	    // Set flag to thumbnail cursor mode
	    cursorMode = 2;
	    
	    //Disable sorting(drag & drop)
	    $("#allThumbnailsOuterDiv").sortable("disable");

	}

    return false;
}

function defaulticon_click() {
    ResetCursorToDefault();
    $('#qc_mainmenu_default').addClass('sbkQc_MainMenuIconCurrent');
    return false;
}

function ResetCursorToDefault()
{
    // Set flag to normal cursor mode
    cursorMode = 1;
    $('#qc_mainmenu_default').removeClass('sbkQc_MainMenuIconCurrent');
    $('#qc_mainmenu_thumb').removeClass('sbkQc_MainMenuIconCurrent');
    $('#qc_mainmenu_move').removeClass('sbkQc_MainMenuIconCurrent');
    $('#qc_mainmenu_delete').removeClass('sbkQc_MainMenuIconCurrent');
    
    // Step through each span
	for (var j = 0; j < spanArray.length; j++)
	{
	    // Get the span
	    var span = $('#' + spanArray[j]);
	    
	    // Reset the cursor on the span
	    span.removeClass('qcPickMainThumbnailCursor');
	    span.removeClass('qcMovePagesCursor');
	    span.removeClass('qcDeletePagesCursor');
	    span.addClass('qcResetMouseCursorToDefault');
	    
	    // Reset each checkbox
	    var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
	    if (checkbox.checked == true) {
	        checkbox.checked = false;
	        span.removeClass('sbkQc_SpanSelected');
	    }

	    checkbox.style.visibility = 'hidden';
	    
	    //Hide all the left/right arrows for moving pages
	    var moveArrows = document.getElementById(spanArray[j].replace('span', 'movePageArrows'));
	    moveArrows.style.visibility = 'hidden';
	}

	   
	//Also re-hide the button for moving multiple pages in case previously made visible
	document.getElementById('divMoveOnScroll').style.visibility = 'hidden';
	document.getElementById('divDeleteMoveOnScroll').style.visibility = 'hidden';
    
    //Re-enable sorting(drag & drop), if the sortable mode selected is 1(enabled) or 2(enabled with confirmation)
    if(makeSortable==1 || makeSortable == 2)
	  $("#allThumbnailsOuterDiv").sortable("enable");
}

//Change cursor: move pages
function movepagesicon_click()
{
    //If this cursor is already set, change back to default
    if (cursorMode == 3) {
        // See if any checkboxes are currently checked
        var checked_found = 0;
        for (var j = 0; j < spanArray.length; j++) {
            var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
            if (checkbox.checked) checked_found++;
        }

        if (checked_found == 0) {
            cancel_move_pages();
        } else {
            update_preview();
            popup('form_qcmove');
        }
    }
    else
    {
        //Remove the default cursor style class first before setting the custom one, 
        //otherwise it will override the custom cursor class
        ResetCursorToDefault();
        
        // Step through each span and set the cursor
        for (var j = 0; j < spanArray.length; j++) {
            // Get the span
            var span = $('#' + spanArray[j]);

            span.removeClass('qcResetMouseCursorToDefault');
            span.addClass('qcMovePagesCursor');
        }
        
        // Set flag to multiple move cursor mode
        cursorMode = 3;
        $('#qc_mainmenu_move').addClass('sbkQc_MainMenuIconCurrent');
        
        //Disable drag & drop of pages
        $("#allThumbnailsOuterDiv").sortable("disable");

        ////Unhide all the checkboxes
        //for (var j = 0; j < spanArray.length; j++) {
        //    var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
        //    checkbox.style.visibility = 'visible';
        //}
    }
    return false;
}


function bulkdeleteicon_click()
{
    //Change the mouse cursor, unhide all the checkboxes
    //If this cursor is already set, change back to default
    if (cursorMode == 4) {
        DeleteSelectedPages();
    }
    else
    {
        //Remove the default cursor style class first before setting the custom one, 
        //otherwise it will override the custom cursor class
        ResetCursorToDefault();
        
        // Step through each span and set the cursor
        for (var j = 0; j < spanArray.length; j++) {
            // Get the span
            var span = $('#' + spanArray[j]);

            span.removeClass('qcResetMouseCursorToDefault');
            span.addClass('qcDeletePagesCursor');
        }
        
        // Set flag to multiple delete cursor mode
        cursorMode = 4;
        $('#qc_mainmenu_delete').addClass('sbkQc_MainMenuIconCurrent');

        //Disable drag & drop of pages while this cursor is set
        $("#allThumbnailsOuterDiv").sortable("disable");

        ////Unhide all the checkboxes
        //for (var j = 0; j < spanArray.length; j++) {
        //    var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
        //    checkbox.style.visibility = 'visible';
        //}
    }

    return false;
}


//Make the thumbnails sortable
function MakeSortable1()
{
    var startPosition;
    var newPosition;

    $("#allThumbnailsOuterDiv").sortable({
        containment: 'parent',
        start: function(event, ui) {
            startPosition = spanArray.indexOf($(ui.item).attr('id'));
        },
        stop: function(event, ui) {
           
// Pull a new spanArray
                var newSpanArray = new Array();
                //get the list of all the thumbnail spans on the page
                spanArrayObjects = $("#allThumbnailsOuterDiv").children();

                //get the spanIDs from the array of span objects 
                var j = 0;
                for (var i = 0; i < spanArrayObjects.length; i++) {
                    if (spanArrayObjects[i].id.indexOf('span') == 0) {
                        newSpanArray[j++] = spanArrayObjects[i].id;
                    }
                }

                var spanID = $(ui.item).attr('id');
                newPosition = newSpanArray.indexOf($(ui.item).attr('id'));



		   //Confirm the move, if sortable option set to 2:'Enabled with confirmation'
            var makeSortable_input = true;
			if(makeSortable==2 && (startPosition!=newPosition))
			{
 //    			alert('startPosition:'+startPosition+' newPosition:'+newPosition);
				makeSortable_input = confirm("Are you sure you want to move this page?");
			}

            if (makeSortable_input == false) {
                $(this).sortable('cancel');
            } else if (makeSortable_input == true) {
                

                // if position has been changed, update the page division correspondingly
                if (startPosition != newPosition) {
                    //get the spanID of the current span being dragged & dropped
                    var pageIndex = spanID.replace('span', '');

                    // Get the two most important spans (one being moved and NEXT after the move)
                    var movedSpan = document.getElementById('newDivType' + pageIndex);
                    var movedFromSpanName = spanArray[startPosition];
                    var movedFromSpanCheckBox = document.getElementById(movedFromSpanName.replace('span', 'newDivType'));

                    //If the span being moved is the start of a new Div 															
                    if (movedSpan.checked == true) {
                        //alert('Moving a new division page');
                        //If the original next div is not the start of a new division, make it the beginning
                        if ((movedFromSpanCheckBox != null) && (movedFromSpanCheckBox.checked == false)) {
                            // Set next original page as new division
                            movedFromSpanCheckBox.checked = true;


                            // Set the division type on the next original page and then set as enabled
                            var divTypeSelectElement = document.getElementById(movedFromSpanName.replace('span', 'selectDivType'));
                            divTypeSelectElement.disabled = false;
                            divTypeSelectElement.value = document.getElementById('selectDivType' + pageIndex).value;

                            //Update the division name textbox
                            if (divTypeSelectElement.value.index('!') == 0) {
                                document.getElementById(movedFromSpanName.replace('span', 'divNameTableRow')).style.visibility = 'visible';
                                document.getElementById(movedFromSpanName.replace('span', 'txtDivName')).disabled = false;
                                document.getElementById(movedFromSpanName.replace('span', 'txtDivName')).value = '';
                            } else {
                                document.getElementById(movedFromSpanName.replace('span', 'divNameTableRow')).style.visibility = 'hidden';
                                document.getElementById(movedFromSpanName.replace('span', 'txtDivName')).disabled = true;
                                document.getElementById(movedFromSpanName.replace('span', 'txtDivName')).value = '';
                            }
                        }
                    }

                    // Since we are done with dealing with the OLD position, we will begin to use the
                    // new array of spans on the page, which reflects the new positioning
                    spanArray = newSpanArray;

                    var movedSpanDivTypeElement = document.getElementById('selectDivType' + pageIndex);

                    //CASE 1: 
                    // If the new position is position 0: This happens if the user moves the page 
                    // to the very beginning of the thumbnails.  In this case, MOVE the div info 
                    // from the first thumbnail to this one.
                    if (newPosition == 0) {
                        // Get the id for the second span (previously the first thumbnail)
                        var previousFirstID = spanArray[1].replace('span', '');

                        //Make the moved div the start of a new div
                        document.getElementById('newDivType' + pageIndex).checked = true;
                        //Enable the moved div's DivType dropdown
                        movedSpanDivTypeElement.disabled = false;

                        //Set the moved div's DivType value to that of the one it is replacing
                        movedSpanDivTypeElement.value = document.getElementById('selectDivType' + previousFirstID).value;

                        //Unmark the replaced div's NewDiv Checkbox (and disable the dropdown)
                        document.getElementById('newDivType' + previousFirstID).checked = false;
                        document.getElementById('selectDivType' + previousFirstID).disabled = true;
                        document.getElementById('txtDivName' + previousFirstID).disabled = true;

                        //If this is now a named div, update the division name textbox
                        if (movedSpanDivTypeElement.value.indexOf('!') == 0) {
                            document.getElementById('divNameTableRow' + pageIndex).style.visibility = 'visible';
                            document.getElementById('txtDivName' + pageIndex).value = document.getElementById('txtDivName' + previousFirstID).value;
                            document.getElementById('txtDivName' + pageIndex).disabled = false;
                        } else {
                            document.getElementById('divNameTableRow' + pageIndex).style.visibility = 'hidden';
                            document.getElementById('txtDivName' + pageIndex).value = '';
                            document.getElementById('txtDivName' + pageIndex).disabled = true;
                        }
                    }

                        //else
                        //CASE 2: Span moved to any location other than 0
                    else if (newPosition > 0) {
                        //Moved span's DivType = preceding Div's Div type
                        movedSpanDivTypeElement.value = document.getElementById(spanArray[newPosition - 1].replace('span', 'selectDivType')).value;

                        // Moved span != start of a new Division
                        document.getElementById('newDivType' + pageIndex).checked = false;
                        movedSpanDivTypeElement.disabled = true;
                        document.getElementById('txtDivName' + pageIndex).disabled = true;

                        //If this is now a named div, update the division name textbox
                        if (movedSpanDivTypeElement.value.indexOf('!') == 0) {
                            document.getElementById('divNameTableRow' + pageIndex).style.visibility = 'visible';
                            document.getElementById('txtDivName' + pageIndex).value = document.getElementById(spanArray[newPosition - 1].replace('span', 'txtDivName')).value;

                        } else {
                            document.getElementById('divNameTableRow' + pageIndex).style.visibility = 'hidden';
                            document.getElementById('txtDivName' + pageIndex).value = '';
                        }
                    } //end else if		
                } //end if(startPosition!=newPosition)
            } //end if(input_box==true)
        }, placeholder: "ui-state-highlight"
    });
							 
    $("#allThumbnailsOuterDiv").disableSelection();
	if(makeSortable==3)
	{
	  $("#allThumbnailsOuterDiv").sortable('disable');
	}
}

//Cancel function: set the hidden field(s) accordingly
function behaviors_cancel_form() 
{
	document.getElementById('QC_behaviors_request').value = 'cancel';
	document.itemNavForm.submit();
	return false;
}

//Save function: set the hidden field(s) accordingly
function behaviors_save_form() 
{
	document.getElementById('QC_behaviors_request').value = 'save';
	document.itemNavForm.submit();
	return false;
}

//Turn On/Off the autosave option
function changeAutoSaveOption()
{
   var linkID = document.getElementById('autosaveLink');
   var hiddenfield = document.getElementById('Autosave_Option');
   var hiddenfield_behavior = document.getElementById('QC_behaviors_request');
	hiddenfield_behavior.value = 'save';

	if(linkID.innerHTML=='Turn Off Autosave')
	{
	  linkID.innerHTML = 'Turn On Autosave';
	  hiddenfield.value = 'false';
//	  alert(hiddenfield.value);
	}
	else
	{
	 linkID.innerHTML = 'Turn Off Autosave';
	 hiddenfield.value = 'true';
	}
	
	//Submit the form
	document.itemNavForm.submit();
	return false;
}

//Auto-save: Called from the main form every three minutes
function qc_auto_save()
{

	jQuery('form').each(function() {
		var hiddenfield = document.getElementById('QC_behaviors_request');
		hiddenfield.value = 'autosave';

		var thisURL =window.location.href.toString();
		// For each form on the page, pass the form data via an ajax POST to
		// the save action
		$.ajax({
					url: thisURL,
					data: 'autosave=true&'+jQuery(this).serialize(),
					type: 'POST',
					async: true,
					success: function(data)
					{
						 //Update the time of saving
						  var currdate = new Date();
						  var hours = currdate.getHours();
						  var minutes = currdate.getMinutes();
						  var ampm = hours >= 12 ? 'PM' : 'AM';

                        //convert from 24 hour format to 12-hour format
						  hours = hours % 12;
						  hours = hours?hours:12;

                        //Append a zero before single digits, for both hours and minutes
						  hours = hours < 10 ? '0' + hours : hours;
						  minutes=minutes<10?'0'+minutes:minutes;
						  var time = hours+":"+minutes+' '+ampm;
						  
						  var timeToDisplay = "Saved at "+time;
				//		  $("#displayTimeSaved").text(timeToDisplay);
							
							return false;
		 
					}// end successful POST function
				}); // end jQuery ajax call
	}); // end setting up the autosave on every form on the page
}


//When any 'bulk move/delete page' checkbox is checked/unchecked
function qccheckbox_onchange(event, chkBoxID)
{
    // This should only happen for cursor modes 3 or 4
    if ((cursorMode != 3) && (cursorMode != 4))
        return;
       
    // Set the background
    if (document.getElementById(chkBoxID).checked == true) {
        $('#' + chkBoxID.replace('chkMoveThumbnail', 'span')).addClass('sbkQc_SpanSelected');
    } else {
        $('#' + chkBoxID.replace('chkMoveThumbnail', 'span')).removeClass('sbkQc_SpanSelected');
    }

    // See if any checkboxes remain checked
    var checked = false;
    if (document.getElementById(chkBoxID).checked == true)
        checked = true;
    else {
        // This isn't checked anymore, are any others?
        for (var i = 0; i < spanArray.length; i++) {
            if (document.getElementById('chkMoveThumbnail' + i).checked == true) {
                checked = true;
                break;
            }
        }
    }

    // If none checked, hide the buttons
    if (!checked) {
        document.getElementById('divMoveOnScroll').style.visibility = 'hidden';
        document.getElementById('divDeleteMoveOnScroll').style.visibility = 'hidden';

        //Hide all the left/right arrows for moving pages
        for (var j = 0; j < spanArray.length; j++) {
            var arrowSpan = document.getElementById(spanArray[j].replace('span', 'movePageArrows'));
            arrowSpan.style.visibility = 'hidden';
        }
    } else {
        
        //If a checkbox has been checked display the necessary controls
        
        // BULK MOVE MODE
        if (cursorMode == 3) {
            document.getElementById('divMoveOnScroll').style.visibility = 'visible';
            for (var j = 0; j < spanArray.length; j++) {
                var arrowSpan = document.getElementById(spanArray[j].replace('span', 'movePageArrows'));
                arrowSpan.style.visibility = 'visible';
            }
        }
        
        // BULK DELETE MODE
        if (cursorMode == 4) {
            document.getElementById('divDeleteMoveOnScroll').style.visibility = 'visible';
            
            }
    }
    
    // Stop the event propogation
    var evt = event ? event : window.event;
    if (evt != null) {
        if (evt.stopPropagation) evt.stopPropagation();
        if (evt.cancelBubble != null) evt.cancelBubble = true;
    }
}


// ------------------ Functions for the Move-Selected-Pages Popup Form---------------------//


//Disable/enable the select dropdowns based on the radio button selected
function rbMovePagesChanged(rbValue)
{
  if(rbValue=='After')
	{
	  document.getElementById('selectDestinationPageList1').disabled=false;
	  document.getElementById('selectDestinationPageList2').disabled=true;
	}
	else if(rbValue=='Before')
	{
	  document.getElementById('selectDestinationPageList2').disabled=false;
	  document.getElementById('selectDestinationPageList1').disabled=true;
	}
    update_preview();
}


//Update the popup form based on the target page filename and relative position passed in
function update_popup_form(page_index,pageID,before_after) {
    page_index_popup = page_index;
   //Uncheck/Check this thumbnail (since clicking on the left/right move arrows for this thumbnail reversed the original user selected option)
        var checkbox = document.getElementById('chkMoveThumbnail' + page_index);
        checkbox.checked = !checkbox.checked;
 //  qccheckbox_onchange(null, 'chkMoveThumbnail' + pageIndex);
   
	 var hidden_request = document.getElementById('QC_behaviors_request');
	 var hidden_action = document.getElementById('QC_move_relative_position');
	 var hidden_destination = document.getElementById('QC_move_destination');
	 var file_name='';
	 
	 hidden_request.value='move_selected_pages';
	 hidden_action.value = '';
	 hidden_destination.value=file_name;
	 
	 

  if(before_after=='After')
  {
	if(document.getElementById('selectDestinationPageList1'))
	{
	 // alert(before_after);
	  document.getElementById('rbMovePages1').checked=true;
	  document.getElementById('selectDestinationPageList1').disabled=false;
	  document.getElementById('selectDestinationPageList2').disabled=true;	
	  //Change the dropdown selected option as well
	  var ddl = document.getElementById('selectDestinationPageList1');
		var opts = ddl.options.length;
		
		for (var i=0; i<opts; i++)
		{
			if (ddl.options[i].text == pageID)
			{
			  ddl.selectedIndex = i;
			}
		}	
	   hidden_action.value = 'After';
	   var ddl=document.getElementById('selectDestinationPageList1');
	   var selIndex = ddl.selectedIndex;
	   hidden_destination.value = ddl.options[selIndex].value;

	  
	}
  }
  else if(before_after=='Before')
  {
	if(document.getElementById('selectDestinationPageList1'))
	{
	  document.getElementById('rbMovePages2').checked=true;
	  document.getElementById('selectDestinationPageList1').disabled=true;
	  document.getElementById('selectDestinationPageList2').disabled=false;	

	  //Change the dropdown selected option as well
	   var ddl = document.getElementById('selectDestinationPageList2');
		var opts = ddl.options.length;
		for (var i=0; i<opts; i++)
		{
			if (ddl.options[i].text == pageID)
			{
				ddl.selectedIndex = i;
				
			}
		}	  
		
		if(document.getElementById('selectDestinationPageList2').selectedIndex>0)
	   { 
		 var ddl=document.getElementById('selectDestinationPageList2');
		 var selIndex = ddl.selectedIndex-1;
		 hidden_action.value = 'After';
		 hidden_destination.value = ddl.options[selIndex].value;

	   }
	   else
	   {
		 hidden_action.value = 'Before';
		 var ddl=document.getElementById('selectDestinationPageList2');
		 hidden_destination.value = ddl.options[ddl.selectedIndex].value;

	   }
		
		
	}
  }
    //Update the 'Preview' section of the popup form
    update_preview();

}

//Move the selected pages
function cancel_move_pages() {

    if ($('#form_qcmove').css('display') != 'none') {
        popdown('form_qcmove');
    }

    ResetCursorToDefault();
    $('#qc_mainmenu_default').addClass('sbkQc_MainMenuIconCurrent');

    // Reset and hide all the checkboxes as well
    for (var j = 0; j < spanArray.length; j++) {
        var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
        if ( checkbox.checked == true )
            checkbox.checked = false;
        //checkbox.style.visibility = 'hidden';
        
        document.getElementById('movePageArrows' + j).style.visibility = 'hidden';
    }

    return false;
}

//Move the selected pages
function move_pages_submit()
{
   // alert('in function move_pages_submit');
	 var hidden_request = document.getElementById('QC_behaviors_request');
	 var hidden_action = document.getElementById('QC_move_relative_position');
	 var hidden_destination = document.getElementById('QC_move_destination');
	 var file_name='';
	 
	 hidden_request.value='move_selected_pages';
	 hidden_action.value = '';
	 hidden_destination.value=file_name;
	 
	 //if 'Before' selected, change to the corresponding 'After' unless 'Before' 0th option is selected
	 if(document.getElementById('rbMovePages2').checked==true)
	 {
	   if(document.getElementById('selectDestinationPageList2').selectedIndex>0)
	   { 
		 var ddl=document.getElementById('selectDestinationPageList2');
		 var selIndex = ddl.selectedIndex-1;
		 hidden_action.value = 'After';
		 hidden_destination.value = ddl.options[selIndex].value;
//	     alert('in the submit function, before selected, changing to after'+hidden_destination.value);
	   }
	   else
	   {
		 hidden_action.value = 'Before';
		 var ddl=document.getElementById('selectDestinationPageList2');
		 hidden_destination.value = ddl.options[ddl.selectedIndex].value;
		
	   }
	   
	 }
	 
	 //else assign the selected 'After' values to the hidden variables
	 else
	 {
	   hidden_action.value = 'After';
	   var ddl=document.getElementById('selectDestinationPageList1');
	   var selIndex = ddl.selectedIndex;
	   hidden_destination.value = ddl.options[selIndex].value;
	//   alert(hidden_destination.value);
	 }
	 
	 document.itemNavForm.submit();
	 return false;
}

//update the preview section of the form
function update_preview() {
    //  alert('function update_preview  called successfully');
    var rb_Before = document.getElementById('rbMovePages2');
    var rb_After = document.getElementById('rbMovePages1');
    var ddl_Before = document.getElementById('selectDestinationPageList2');
    var ddl_After = document.getElementById('selectDestinationPageList1');
    var prevThumbnail = document.getElementById('prevThumbnailImage');
    var nextThumbnail = document.getElementById('nextThumbnailImage');
    var placeholderImage1 = document.getElementById('PlaceholderThumbnailImage1');
    var placeholderImage2 = document.getElementById('PlaceholderThumbnailImage2');
    var placeholderImage3 = document.getElementById('PlaceholderThumbnailImage3');

    //set the placeholder images
//    placeholderImage1.src = qc_image_folder + "move_pages_here.jpg";
//    placeholderImage2.src = qc_image_folder + "move_pages_here.jpg";
//    placeholderImage3.src = qc_image_folder + "move_pages_here.jpg";

  //If the 'Before' radio button is selected
    if (rb_Before.checked == true) {
        //if before the 0th option is selected
        if (ddl_Before.selectedIndex == 0) {
            //set the prevThumbnail image to 'No pages'
            prevThumbnail.src = qc_image_folder + "no_pages.jpg";
            document.getElementById('prevFileName').innerHTML = '';
        }
            
        else {// set the prev thumbnail
            if (ddl_Before.options[ddl_Before.selectedIndex - 1].value.length > 0 && thumbnailImageDictionary[ddl_Before.options[ddl_Before.selectedIndex - 1].value].length > 0) {
                prevThumbnail.src = thumbnailImageDictionary[ddl_Before.options[ddl_Before.selectedIndex - 1].value];
                document.getElementById('prevFileName').innerHTML = ddl_Before.options[ddl_Before.selectedIndex-1].value;
            }
        }
        //set the nextThumbnail
            if (ddl_Before.options[ddl_Before.selectedIndex].value.length>0 && thumbnailImageDictionary[ddl_Before.options[ddl_Before.selectedIndex].value].length>0) {
                nextThumbnail.src = thumbnailImageDictionary[ddl_Before.options[ddl_Before.selectedIndex].value];
                document.getElementById('nextFileName').innerHTML = ddl_Before.options[ddl_Before.selectedIndex].value;
            } else {
                nextThumbnail.src = qc_image_folder + "no_pages.jpg";
                document.getElementById('nextFileName').innerHTML = '';
            }
        
    }
    else if (rb_After.checked == true) {
        //if after the 'last' option is selected
        if (ddl_After.selectedIndex == ddl_After.options.length-1) {
            //set the nextThumbnail image to 'No pages'
            nextThumbnail.src = qc_image_folder + "no_pages.jpg";
            document.getElementById('nextFileName').innerHTML = '';
        }

        else {// set the next thumbnail
            if (ddl_After.options[ddl_After.selectedIndex + 1].value.length > 0 && thumbnailImageDictionary[ddl_After.options[ddl_After.selectedIndex + 1].value].length > 0) {
                nextThumbnail.src = thumbnailImageDictionary[ddl_After.options[ddl_After.selectedIndex + 1].value];
                document.getElementById('nextFileName').innerHTML = ddl_After.options[ddl_After.selectedIndex + 1].value;
            }
            else {
                nextThumbnail.src = qc_image_folder + "no_pages.jpg";
                document.getElementById('nextFileName').innerHTML = '';
            }
        }
        //set the prevThumbnail
        if (ddl_After.options[ddl_After.selectedIndex].value.length > 0 && thumbnailImageDictionary[ddl_After.options[ddl_After.selectedIndex].value].length > 0) {
            prevThumbnail.src = thumbnailImageDictionary[ddl_After.options[ddl_After.selectedIndex].value];
            document.getElementById('prevFileName').innerHTML = ddl_After.options[ddl_After.selectedIndex].value;
        } else {
            prevThumbnail.src = qc_image_folder + "no_pages.jpg";
            document.getElementById('prevFileName').innerHTML = '';
        }

    }

    //Adjust the placeholder span(s) based on the number of pages(checkboxes) selected 
    var chkboxCount = 0;
    var placeHolderSpan1 = document.getElementById('PlaceholderThumbnail1');
    var placeHolderSpan2 = document.getElementById('PlaceholderThumbnail2');
    var placeHolderSpan3 = document.getElementById('PlaceholderThumbnail3');
    var firstCheckedFileName;


    for (var j = 0; j < spanArray.length; j++) {
        var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
        if (page_index_popup.length > 0 && document.getElementById('chkMoveThumbnail' + page_index_popup).checked == true && page_index_popup == (spanArray[j].replace('span', ''))) {
            page_index_popup = '';
            continue;
            
        }
        else if (page_index_popup.length > 0 && document.getElementById('chkMoveThumbnail' + page_index_popup).checked == false && page_index_popup == (spanArray[j].replace('span', ''))) {
            chkboxCount++;
  //          alert(page_index_popup);
            if (chkboxCount == 1)
                firstCheckedFileName = document.getElementById('filename' + page_index_popup).value;
        }

        if (checkbox.checked == true) {
            chkboxCount++;
            if (chkboxCount == 1) {
                firstCheckedFileName = document.getElementById(spanArray[j].replace('span', 'filename')).value;
 //               alert(firstCheckedFileName);

            }
        }
    }
    //  alert(firstCheckedFileName);
    if (chkboxCount == 1) {
        placeHolderSpan1.style.visibility = 'hidden';
        placeHolderSpan2.style.visibility = 'hidden';
        //set the placeholder images
    //    placeholderImage1.src = qc_image_folder + "move_page_here.jpg";
    //    placeholderImage2.src = qc_image_folder + "move_page_here.jpg";
   //     placeholderImage3.src = qc_image_folder + "move_page_here.jpg";



    }
    
    else if (chkboxCount > 1) {
        placeHolderSpan1.style.visibility = 'visible';
        placeHolderSpan2.style.visibility = 'visible';
        //set the placeholder images
    //    placeholderImage1.src = qc_image_folder + "move_pages_here.jpg";
   //     placeholderImage2.src = qc_image_folder + "move_pages_here.jpg";
        //     placeholderImage3.src = qc_image_folder + "move_pages_here.jpg";

    }
    placeholderImage1.src = thumbnailImageDictionary[firstCheckedFileName];
    placeholderImage2.src = thumbnailImageDictionary[firstCheckedFileName];
    placeholderImage3.src = thumbnailImageDictionary[firstCheckedFileName];

    document.getElementById('placeHolderText1').innerHTML = firstCheckedFileName;
    document.getElementById('placeHolderText2').innerHTML = firstCheckedFileName;
    document.getElementById('placeHolderText3').innerHTML = firstCheckedFileName;


}


//--------------------End of Functions for the Move-Selected-Pages Popup Form----------------//


function ImageDeleteClicked(Filename) {
	var input_box = confirm("Are you sure you want to delete this page and apply all changes up to this point?");
	if (input_box == true) {
	    document.getElementById('QC_behaviors_request').value = 'delete_page';
	    document.getElementById('QC_affected_file').value = Filename;
		document.itemNavForm.submit();
	}
	return false;
}


function DeleteSelectedPages() {
    
    // See if any checkboxes are currently checked
    var checked_found = 0;
    for (var j = 0; j < spanArray.length; j++) {
        var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
        if (checkbox.checked) checked_found++;
    }

    if (checked_found == 0) {
        ResetCursorToDefault();
        $('#qc_mainmenu_default').addClass('sbkQc_MainMenuIconCurrent');

        // Reset and hide all the checkboxes as well
        for (var j = 0; j < spanArray.length; j++) {
            var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
            if ( checkbox.checked == true )
                checkbox.checked = false;
            //checkbox.style.visibility = 'hidden';
        }

    } else {
        var input_box = confirm("Are you sure you want to delete these " + checked_found + " pages and apply all changes up to this point?");
        if (input_box == true) {

            document.getElementById('QC_behaviors_request').value = 'delete_selected_pages';
            document.itemNavForm.submit();
        } else {
            ResetCursorToDefault();
            $('#qc_mainmenu_default').addClass('sbkQc_MainMenuIconCurrent');

            // Reset and hide all the checkboxes as well
            for (var j = 0; j < spanArray.length; j++) {
                var checkbox = document.getElementById(spanArray[j].replace('span', 'chkMoveThumbnail'));
                if (checkbox.checked == true) {
                    checkbox.checked = false;
                    $('#' + spanArray[j]).removeClass('sbkQc_SpanSelected');
                }

                //checkbox.style.visibility = 'hidden';
            }
        }
    }
    
	return false;
}

function thumbnail_click(spanid, url) {
    if (cursorMode == 1) {
        window.open(url, spanid);
    }
}

// Function is called when user clicks COMPLETE
function save_submit_form() {
    document.getElementById('QC_behaviors_request').value = 'complete';
	document.itemNavForm.submit();
	return false;
}
 
//Set the appropriate hidden variable for post back when the user selects the Clear_Pagination option
function ClearPagination() {
    document.getElementById('QC_behaviors_request').value = 'clear_pagination';
    document.itemNavForm.submit();
    return false;
}


//Set the appropriate hidden variable for post back when the user selects the Clear_Pagination option
function ClearReorderPagination() {
    document.getElementById('QC_behaviors_request').value = 'clear_reorder';
    document.itemNavForm.submit();
    return false;
}

function UploadNewPageImages(url) {
    var input_box = confirm("Are you sure you want to add more page images?   Any unsaved changes will be lost.");
    if (input_box == true) {
        window.location.href = url;
    }
    return false;
}

// Function called when the user mouses over a page
function qcspan_mouseover(spanid) {
    $('#' + spanid).removeClass('sbkQc_SpanMouseOut');
    $('#' + spanid).addClass('sbkQc_SpanMouseOver');
    showQcPageIcons(spanid);
    showErrorIcon(spanid);
    return false;
}

// Function called when the user mouses out of a page
function qcspan_mouseout(spanid) {
    $('#' + spanid).removeClass('sbkQc_SpanMouseOver');
    $('#' + spanid).addClass('sbkQc_SpanMouseOut');
    hideQcPageIcons(spanid);
    hideErrorIcon(spanid);
    return false;
}

function qcspan_onclick(event, spanid) {
   
    // Get the page index
    var pageIndex = spanid.replace('span', '');
    
    //Cursor currently set to the "Pick Main Thumbnail" cursor?
    if (cursorMode == 2) {

        // Set the hidden value
        document.getElementById('Main_Thumbnail_File').value = document.getElementById('filename' + pageIndex).value;

        // Reset the cursor
        ResetCursorToDefault();
        $('#qc_mainmenu_default').addClass('sbkQc_MainMenuIconCurrent');

        // Ensure no other spans are set
        for (var i = 0; i < spanArray.length; i++) {
            if (document.getElementById(spanArray[i].replace('span', 'pick_main_thumbnail')).style.visibility == 'visible') {
                document.getElementById(spanArray[i].replace('span', 'pick_main_thumbnail')).style.visibility = 'hidden';
                break;
            }
        }

        // Set the new thumbnail in the user interface
        document.getElementById('pick_main_thumbnail' + pageIndex).style.visibility = 'visible';
    }
    
    // Cursor set to bulk delete or move
    if ((cursorMode == 3) || (cursorMode == 4)) {

        var checkbox = document.getElementById('chkMoveThumbnail' + pageIndex);
        checkbox.checked = !checkbox.checked;
        if (event.shiftKey) {
            if ((checkbox.checked) && (lastSelected >= 0)) {


                var thisIndex = spanArray.indexOf(spanid);

                var start = Math.min(thisIndex, lastSelected);
                var end = Math.max(thisIndex, lastSelected);

                for (var i = start; i < end; i++) {
                    document.getElementById(spanArray[i].replace('span', 'chkMoveThumbnail')).checked = true;
                    $('#' + spanArray[i]).addClass('sbkQc_SpanSelected');
                }
            }
        } else {
            if (checkbox.checked) {
                lastSelected = spanArray.indexOf(spanid);
            } else {
                lastSelected = -1;
            }
        }

        // Always fire this event, for some more global changes
        qccheckbox_onchange(null, 'chkMoveThumbnail' + pageIndex);
    }
}



