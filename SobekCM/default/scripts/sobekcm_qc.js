//Update the division types when a division checkbox is checked/unchecked
function UpdateDivDropdown(CheckBoxID, MaxPageCount)
{
	//get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	if(window.spanArrayGlobal!= null)
	 { 
	
	  spanArrayObjects = window.spanArrayGlobal;
	 
	 }
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }
     

	if(document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).disabled==true)
	{
	  document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).disabled=false;
	  
	  //If a division name textbox is required and disabled, enable it
	  document.getElementById('txtDivName'+(CheckBoxID.split('newdiv')[1])).disabled = false;
	}
	else
	{
	 document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).disabled=true;
	 //Disable the named division textbox
	 if(document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).value.indexOf('!')==0)
	   document.getElementById('txtDivName'+(CheckBoxID.split('newdiv')[1])).disabled = true;
	 
	 //update the subsequent divs
	 var index=CheckBoxID.split('newdiv')[1];
	 var i = spanArray.indexOf(index);

	  if(i==0)
	  {
		document.getElementById('selectDivType'+index).value='Main';
		
//		while(document.getElementById('selectDivType'+spanArray[i]).disabled==true)
       while(i<spanArray.length)
		{
          i++;
		  if(document.getElementById('selectDivType'+spanArray[i]))
			{
			  document.getElementById('selectDivType'+spanArray[i]).value = 'Main';
			  document.getElementById('newDivType'+spanArray[i]).checked=false;
              document.getElementById('newDivType'+spanArray[i]).disabled=true;

			}
		
		}
	
	  }
	  else
	  {
		var j=i-1;
		var valToSet = 'Main';

		while(!(document.getElementById('selectDivType'+spanArray[j])))
		{
		 j--;
		}

		valToSet = document.getElementById('selectDivType'+spanArray[j]).value;

		var k=i;
		  
		//set the page division type of all pages till the start of the next div
		while(document.getElementById('selectDivType'+spanArray[k]).disabled==true && k<spanArray.length)
		{
		  if(document.getElementById('selectDivType'+spanArray[k]))
			document.getElementById('selectDivType'+spanArray[k]).value = valToSet;
		  k++;
		}
		
	  }
	  
	}

}

//Change all subsequent division types when one div type is changed. Also update the named division type textboxes as appropriate
function DivisionTypeChanged(selectID,MaxPageCount)
{
   //Get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	//Get the array from the global variable if previously assigned
	if(window.spanArrayGlobal!= null)
	 { 
	  spanArrayObjects = window.spanArrayGlobal;
	 }
	 //else set the array values here
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }

	var currID = selectID.split('selectDivType')[1];
   // var i=parseInt(currID)+1;
    var i = spanArray.indexOf(currID)+1;
	var currVal = document.getElementById(selectID).value;


	//if the new Division type selected is a nameable div
	if(currVal.indexOf('!')==0)
	{
	  document.getElementById('divNameTableRow'+currID).className='txtNamedDivVisible';

	 var j=i;
	 //Make the name textboxes of all other pages of this div visible
	 while(document.getElementById('selectDivType'+spanArray[j]).disabled==true)
  	 {
	  //alert(i);
	  if(document.getElementById('selectDivType'+ spanArray[j]))
	   { 
	     document.getElementById('selectDivType'+ spanArray[j]).value = currVal;
		 document.getElementById('txtDivName'+spanArray[j]).disabled = true;
	   }
	  if(document.getElementById('divNameTableRow'+ spanArray[j]))
		document.getElementById('divNameTableRow'+ spanArray[j]).className = 'txtNamedDivVisible';
	  j++;
	 }


	}
	
	//else if the division type selected is not a nameable div
	else if(currVal.indexOf('!')==-1)
	{
	 //Hide the name textbox for this page
	 document.getElementById('divNameTableRow'+currID).className='txtNamedDivHidden';
	 var j=i;
	
	//Hide the name textboxes of all the other pages of this division type
    while(document.getElementById('selectDivType'+spanArray[j]).disabled==true)
  	 {
	  //alert(i);
	  
	 if(document.getElementById('selectDivType'+ spanArray[j]))
	    document.getElementById('selectDivType'+ spanArray[j]).value = currVal;
	  if(document.getElementById('divNameTableRow'+ spanArray[j]))
		document.getElementById('divNameTableRow'+ spanArray[j]).className = 'txtNamedDivHidden';
	  j++;
	 }
	}
	

}


//Update the division name through the textboxes of the division type when changed in one
function DivNameTextChanged(textboxID, MaxPageCount)
{
   //Get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	//Get the array of all UI thumbnails from the global variable if previously assigned
	if(window.spanArrayGlobal!= null)
	 { 
	  spanArrayObjects = window.spanArrayGlobal;
	 }
	 //else set the array values here
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }
     var currVal = document.getElementById(textboxID).value;
     var currID = textboxID.split('txtDivName')[1];
  
  //Update the textboxes of the same division after this one
  var i=spanArray.indexOf(currID)+1;

  while(document.getElementById('selectDivType'+spanArray[i]).disabled==true)
  {
     document.getElementById('txtDivName'+spanArray[i]).value = currVal;
	 i++;
  }
  //If updated somewhere in the middle of the div, also update the previous textboxes all the way till the start of the current division
  if(document.getElementById('selectDivType'+currID).disabled==true)
  { 
    i=spanArray.indexOf(currID)-1;
	while(i>0 && document.getElementById('selectDivType'+spanArray[i]).disabled==true)
	{
	  document.getElementById('txtDivName'+spanArray[i]).value = currVal;
	  i--;
	}
	document.getElementById('txtDivName'+spanArray[i]).value = currVal;
  }
  
  

}


//Autonumber subsequent textboxes on changing one textbox value
function PaginationTextChanged(textboxID,mode,MaxPageCount)
{

    //get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	if(window.spanArrayGlobal!= null)
	 { 
	  spanArrayObjects = window.spanArrayGlobal;
	 }
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }

  //Mode '0': Autonumber all the thumbnail page names till the end
  if(mode=='0')
  {
    
    var matches = document.getElementById(textboxID).value.match(/\d+/g);
    if (matches != null) 
    {
       // the id attribute contains a digit
       var len=matches.length;
       var number = matches[len-1];
	   var nonNumber='';
	   var val=document.getElementById(textboxID).value;
      // alert(number);
       
	   //if the number is at the end of the string, with a space before
	   if(val.indexOf(number.toString())==(val.length-number.toString().length) && val.substr(val.indexOf(number.toString())-1,1)==' ')
       {
      //      for(var i=parseInt(textboxID.split('textbox')[1])+1;i<=MaxPageCount;i++)
			for(var i=spanArray.indexOf(textboxID.split('textbox')[1])+1;i<=MaxPageCount;i++)
			{
			  number++;
			 //alert(i);
			  if(document.getElementById('textbox'+spanArray[i]))
			  {
			    document.getElementById('textbox'+spanArray[i]).value = 
				 document.getElementById(textboxID).value.substr(0,(document.getElementById(textboxID).value.length-number.toString.length)-1)+' '+number.toString();
			  }//end if
			}//end for
           
       }//end if
    }//end if


  }//end if
 
 //Mode '1': Autonumber all the thumbnail pages till the start of the next div
   if(mode=='1')
  {
    
    var matches = document.getElementById(textboxID).value.match(/\d+/g);
    if (matches != null) 
    {
       // the id attribute contains a digit
       var len=matches.length;
       var number = matches[len-1];
	   var nonNumber='';
	   var val=document.getElementById(textboxID).value;
      // alert(number);
       
	   //if the number is at the end of the string, with a space before
	   if(val.indexOf(number.toString())==(val.length-number.toString().length) && val.substr(val.indexOf(number.toString())-1,1)==' ')
       {
      //      for(var i=parseInt(textboxID.split('textbox')[1])+1;i<=MaxPageCount;i++)
			var i=spanArray.indexOf(textboxID.split('textbox')[1])+1;
			while(document.getElementById('selectDivType'+spanArray[i]).disabled==true && i<MaxPageCount)
			{
			  number++;
			 //alert(i);
			  if(document.getElementById('textbox'+spanArray[i]))
			  {
			    document.getElementById('textbox'+spanArray[i]).value = 
				 document.getElementById(textboxID).value.substr(0,(document.getElementById(textboxID).value.length-number.toString.length)-1)+' '+number.toString();
			  }//end if
			  i++;
			}//end while
           
       }//end if
    }//end if


  }//end if
  
  
}//end function


//Assign the 'main thumbnail' to the selected thumbnail span
function PickMainThumbnail(spanID)
{
	var pageIndex = spanID.split('span')[1];
	var hiddenfield = document.getElementById('Main_Thumbnail_Index');
	var hidden_request = document.getElementById('QC_behaviors_request');
	 hidden_request.value="";

	
	//Cursor currently set to the "Pick Main Thumbnail" cursor?
	if($('body').css('cursor').indexOf("thumbnail_cursor")>-1)
	{
	  var spanImageID='spanImg'+pageIndex;
	  //is there a previously selected Main Thumbnail?
	  if(hiddenfield.length>0 && document.getElementById('spanImg'+hiddenfield).className=="pickMainThumbnailIconSelected")
	  {
	    //First unmark the existing one as the main thumbnail
		document.getElementById('spanImg'+hiddenfield).className='pickMainThumbnailIcon';
		
		//Then set the hidden request value to 'unpick'
		hidden_request.value='unpick_main_thumbnail';
							
	  }
	  
	  //User selects a main thumbnail
	  if(document.getElementById(spanImageID).className=="pickMainThumbnailIcon")
	  {
		document.getElementById(spanImageID).className="pickMainThumbnailIconSelected";
		
		//Remove the current cursor class
		$('body').removeClass('qcPickMainThumbnailCursor');
		//Change the cursor back to default
		$('body').addClass('qcResetMouseCursorToDefault');
		
		//Set the hidden field value with the main thumbnail
		hiddenfield.value = pageIndex;
	    hidden_request.value = "pick_main_thumbnail";

		
	  }
	  else
	  {
	  //Confirm if the user wants to unmark this as a thumbnail image
	  
		//   var t=confirm('Are you sure you want to remove this as the main thumbnail?');   
		 // if(t==true)
		  {
			  document.getElementById(spanImageID).className = "pickMainThumbnailIcon";
			 //Change the cursor back to default
			 $('body').addClass('qcResetMouseCursorToDefault');
			 
			 //Set the hidden field value with the main thumbnail
			 
			 hiddenfield.value = pageIndex;
	         hidden_request.value = 'unpick_main_thumbnail';  
			 
			 			 
		  }
	  }
	  // Submit this
	  document.itemNavForm.submit();
	  return false;
	  
	}

}

//Show the QC Icons below the thumbnail on mouseover
function showQcPageIcons(spanID)
{
  //alert(spanID);
  var pageIndex = spanID.split('span')[1];
  var qcPageIconsSpan = 'qcPageOptions'+pageIndex;
  document.getElementById(qcPageIconsSpan).className = "qcPageOptionsSpanHover";
}

//Hide the QC Icon bar below the thumbnail on mouseout
function hideQcPageIcons(spanID)
{
  var pageIndex = spanID.split('span')[1];
  var qcPageIconsSpan = 'qcPageOptions'+pageIndex;
  document.getElementById(qcPageIconsSpan).className = "qcPageOptionsSpan";
}

//Show the error icon on mouseover
function showErrorIcon(spanID)
{
  var pageIndex = spanID.split('span')[1];
  var qcErrorIconSpan = 'error'+pageIndex;
  document.getElementById(qcErrorIconSpan).className = "errorIconSpanHover";
}

//Hide the error icon on mouseout
function hideErrorIcon(spanID)
{
  var pageIndex = spanID.split('span')[1];
  var qcErrorIconSpan = 'error'+pageIndex;
  document.getElementById(qcErrorIconSpan).className = "errorIconSpan";

}

//Change the cursor to the custom cursor for Selecting a Main Thumbnail
function ChangeMouseCursor(MaxPageCount)
{

	//Remove the default cursor style class, and any other custom class first before setting this one, 
	//otherwise it will override the custom cursor class
	$('body').removeClass('qcResetMouseCursorToDefault');
    $('body').removeClass('qcMovePagesCursor');
	//Set the custom cursor
	$('body').addClass('qcPickMainThumbnailCursor');

		//Clear and hide all the 'move' checkboxes, in case currently visible
		for(var i=0;i<MaxPageCount; i++)
		{
		  if(document.getElementById('chkMoveThumbnail'+i))
		  {
			  document.getElementById('chkMoveThumbnail'+i).checked=false;
			  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailHidden';
		  }
		
		}

	//Also re-hide the button for moving multiple pages in case previously made visible
	document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
	
	 //Hide all the left/right arrows for moving pages
	for(var i=0; i<MaxPageCount; i++)
	{
			 if(document.getElementById('movePageArrows'+i))
			   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
		
	}
		
}

function ResetCursorToDefault(MaxPageCount)
{
	//Remove custom cursor classes if any
	$('body').removeClass('qcPickMainThumbnailCursor');
	$('body').removeClass('qcMovePagesCursor');

	//Reset to default
	$('body').addClass('qcResetMouseCursorToDefault');
	
	//Clear and hide all the 'move' checkboxes, in case currently visible
	for(var i=0;i<MaxPageCount; i++)
	{
	  if(document.getElementById('chkMoveThumbnail'+i))
	  {
		  document.getElementById('chkMoveThumbnail'+i).checked=false;
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailHidden';
	  }
	
	}
	//Also re-hide the button for moving multiple pages in case previously made visible
	document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
	
	 //Hide all the left/right arrows for moving pages
	for(var i=0; i<MaxPageCount; i++)
	{
			 if(document.getElementById('movePageArrows'+i))
			   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
		
	}
}

//Change cursor: move pages
function MovePages(MaxPageCount)
{

//Remove the default cursor style class first before setting the custom one, 
//otherwise it will override the custom cursor class
$('body').removeClass('qcResetMouseCursorToDefault');
$('body').removeClass('qcPickMainThumbnailCursor');

//First change the cursor
$('body').addClass('qcMovePagesCursor');

   //Unhide all the checkboxes
	for(var i=0;i<MaxPageCount; i++)
	{
		if(document.getElementById('chkMoveThumbnail'+i))
		{
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailVisible';
		}
	}

}


//Make the thumbnails sortable
function MakeSortable1()
{

 var startPosition;
 var newPosition; 
 var oldArray;
 var newArray;

$("#allThumbnailsOuterDiv").sortable({containment: 'parent',
											start: function(event, ui)
                                             {
											   startPosition=$(ui.item).index()+1;
											 },
                                             stop: function(event, ui)
									         {
												   newPosition = $(ui.item).index()+1;
																								  
												
												//get the list of all the thumbnail spans on the page
												 var spanArrayObjects = $(ui.item).parent().children();
												 
												 var spanArray=new Array();
												
												 //get the spanIDs from the array of span objects 
												 for(var i=0;i<spanArrayObjects.length;i++)
												 {
												   spanArray[i]=spanArrayObjects[i].id;
												 }
																								
												//save the array of spans in the UI as a global window variable
												 window.spanArrayGlobal = spanArray;
												
												//if position has been changed, update the page division correspondingly
												  if(startPosition != newPosition)
												  {
												    //get the spanID of the current span being dragged & dropped
													 var spanID=$(ui.item).attr('id');
													 var pageIndex = spanID.split('span')[1];
													
													
												   //get the current index of the moved span in the UI spanArray
													var indexSpanArray = spanArray.indexOf(spanID);   												
													   
													var nextPosition=spanArray[indexSpanArray+1].split('span')[1];
																										
													var indexTemp = spanArray[startPosition].split('span')[1];
													
													//If the span being moved is the start of a new Div 															
													if (document.getElementById('newDivType' + spanArray[newPosition-1].split('span')[1]).checked == true)
													{
													   //alert('Moving a new division page');

                                                        //If the next div is not the start of a new division 
													   if (document.getElementById('newDivType' + (spanArray[startPosition].split('span')[1])).checked == false)
													   {
													        document.getElementById('newDivType' + (spanArray[startPosition].split('span')[1])).checked = true;
													        document.getElementById('selectDivType' + (spanArray[startPosition].split('span')[1])).disabled = false;
													        //alert('still in the right place');
													        document.getElementById('selectDivType' + (spanArray[startPosition].split('span')[1])).value = document.getElementById('selectDivType' + pageIndex).value;

													        //Update the division name textbox
													        if (document.getElementById('selectDivType' + (spanArray[startPosition].split('span')[1])).value.index('!')==0)
													        {
													            document.getElementById('divNameTableRow' + (spanArray[startPosition].split('span')[1])).className = 'txtNamedDivVisible';
													            document.getElementById('txtDivName' + (spanArray[startPosition].split('span')[1])).disabled = false;

													        }
													        else
													        {
													            document.getElementById('txtDivName' + (spanArray[startPosition].split('span')[1])).disabled = true;
													        }
													   }
													    //else do nothing
													}
													//else do nothing
													
													//CASE 1: 
													//If the new position is position 0: Theoretically this cannot happen since the sortable list container boundary
													//set makes this impossible, but just in case.
													if(indexSpanArray==0)
													{
							                          //Make the moved div the start of a new div
													  document.getElementById('newDivType'+(spanArray[newPosition-1].split('span')[1])).checked=true;
													  //Enable the moved div's DivType dropdown
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).disabled=false;
													  //Set the moved div's DivType value to that of the one it is replacing
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).value = document.getElementById('selectDivType'+(spanArray[newPosition].split('span')[1])).value;
													  
													  //Unmark the replaced div's NewDiv Checkbox (and disable the dropdown)
													  document.getElementById('newDivType'+(spanArray[newPosition].split('span')[1])).checked=false;
													  document.getElementById('selectDivType'+(spanArray[newPosition].split('span')[1])).disabled=true;
	                                                  
													  //If this is now a named div, update the division name textbox
													  if(document.getElementById('selectDivType'+(spanArray[newPosition].split('span')[1])).value.IndexOf('!')==0)
													  {
													      document.getElementById('divNameTableRow' + (spanArray[newPosition].split('span')[1])).className = 'txtNamedDivVisible';
														document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1])).value = document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1]+1)).value;
														document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1]+1)).disabled=true;
													  }
													  else
													  {
													      document.getElementById('divNameTableRow' + (spanArray[newPosition].split('span')[1])).className = 'txtNamedDivHidden';
														document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1]+1)).disabled=false;
													  }
													}
													
													//else
													//CASE 2: Span moved to any location other than 0
																									
													else if (indexSpanArray > 0)
													{
													 //Moved span's DivType = preceding Div's Div type
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).value = document.getElementById('selectDivType'+(spanArray[newPosition-2].split('span')[1])).value;
													  //Moved span != start of a new Division
													  document.getElementById('newDivType'+(spanArray[newPosition-1].split('span')[1])).checked=false;
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).disabled=true;
													  
													  //update the division name textbox
													  if(document.getElementById('selectDivType'+(spanArray[newPosition-2].split('span')[1])).value.indexOf('!')==0)
													  {
													    document.getElementById('divNameTableRow' + (spanArray[newPosition - 1].split('span')[1])).className = 'txtNamedDivVisible';
														document.getElementById('txtDivName'+(spanArray[newPosition-1].split('span')[1])).disabled=true;
													    document.getElementById('txtDivName'+(spanArray[newPosition-1].split('span')[1])).value=document.getElementById('txtDivName'+(spanArray[newPosition-2].split('span')[1])).value;
														
													  }
													  else
													  {
													    document.getElementById('divNameTableRow' + (spanArray[newPosition - 1].split('span')[1])).className = 'txtNamedDivHidden';
														document.getElementById('txtDivName'+(spanArray[newPosition-1].split('span')[1])).disabled=false;
													  }
													  
													  
													}//end else if
											 
													 
												 }//end if(startPosition!=newPosition)
											  

											 },placeholder: "ui-state-highlight"});
									 
$("#allThumbnailsOuterDiv").disableSelection();


                                                 															 

}



//Cancel function: set the hidden field(s) accordingly
function behaviors_cancel_form() 
{
	var hiddenfield = document.getElementById('QC_behaviors_request');
	hiddenfield.value = 'cancel';

	
    // Submit this
    document.itemNavForm.submit();
    return false;
}


//Save function: set the hidden field(s) accordingly
function behaviors_save_form() 
{
    var hiddenfield = document.getElementById('QC_behaviors_request');
	hiddenfield.value = 'save';
	
    // Submit this
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

//Autosave the QC form. Called from the main form every three minutes
function qc_auto_save()
{

	jQuery('form').each(function() {
	    var hiddenfield = document.getElementById('QC_behaviors_request');
		hiddenfield.value = 'save';

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
						  
	//						alert('Autosaving...');
							return false;
		 
					}// end successful POST function
				}); // end jQuery ajax call
    }); // end setting up the autosave on every form on the page
}


//When any 'move page' checkbox is is checked/unchecked
function chkMoveThumbnailChanged(chkBoxID, MaxPageCount)
{

  var checked=false;
 document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
 //Hide all the left/right arrows for moving pages
for(var i=0; i<MaxPageCount; i++)
{
		 if(document.getElementById('movePageArrows'+i))
		   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
	
}
 
 
 //If a checkbox has been checked
 if (document.getElementById(chkBoxID).checked==true)
 {
    document.getElementById('divMoveOnScroll').className='qcDivMoveOnScroll';
    for(var i=0; i<MaxPageCount; i++)
	{
		 if(document.getElementById('movePageArrows'+i))
		   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconVisible';
	
	}
}
else
{ 
  //Check if there is any other checked checkbox on the screen
  for(var i=0; i<MaxPageCount; i++)
  {
    if((document.getElementById('chkMoveThumbnail'+i)) && document.getElementById('chkMoveThumbnail'+i).checked==true)
	{
	  document.getElementById('divMoveOnScroll').className='qcDivMoveOnScroll';
	  checked = true;
	}
  }
  
  if(checked==true)
  {
     //Unhide the left/right arrows for moving pages
     for(var i=0; i<MaxPageCount; i++)
	{
		 if(document.getElementById('movePageArrows'+i))
		   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconVisible';
	
	}
  
  }

}
  
}


// ------------------ Functions for the Move-Multiple-Selected-Pages Popup Form---------------------//


//Disable\enable the select dropdowns based on the radio button selected
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
}


//Update the popup form based on the parameters passed in
function update_popup_form(pageID,before_after)
{
  //alert(pageID+before_after);
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
	}
  }
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
	 
	 //if 'Before' selected, change to corresponding 'After' unless 'Before' 0th option is selected
	 if(document.getElementById('rbMovePages2').checked==true)
	 {
	   if(document.getElementById('selectDestinationPageList2').selectedIndex>0)
	   { 
	     var ddl=document.getElementById('selectDestinationPageList2');
	     var selIndex = ddl.selectedIndex-1;
		 hidden_action.value = 'After';
		 hidden_destination.value = ddl.options[selIndex].value;
	   //  alert(hidden_destination.value);
	   }
	   else
	   {
	     hidden_action.value = 'Before';
		 var ddl=document.getElementById('selectDestinationPageList2');
		 hidden_destination.value = ddl.options[ddl.selectedIndex].value;
		// alert(hidden_destination.value);
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


//--------------------End of Functions for the Move-Multiple-Selected-Pages Popup Form----------------//


function ImageDeleteClicked(filename) {
    var hidden_request = document.getElementById('QC_behaviors_request');
    var details = document.getElementById('QC_affected_file');

    hidden_request.value = 'delete_page';
    details.value = filename;
    
    document.itemNavForm.submit();
    return false;
}