//Global variables
var page = 1;



//Set the global page value: this indicates the current tab selected
function setCurrentTab(thisPage) {
   
    page = thisPage;
//    alert(page);
}


//Tab 1 - With Duration
//Hide/Unhide the item info fields based on the entry type selected - Barcode/Manual
function rbEntryTypeChanged(value) {
    if (value == 0) {
        
        document.getElementById("tblrow_Barcode").style.display = 'table-row';
        document.getElementById("tblrow_Manual1").style.display = 'none';
        document.getElementById("tblrow_Manual2").style.display = 'none';
      

    }
    else
    {
        
        document.getElementById("tblrow_Barcode").style.display = 'none';
        document.getElementById("tblrow_Manual1").style.display = 'table-row';
        document.getElementById("tblrow_Manual2").style.display = 'table-row';
    }
}


//Tab2 - Without Duration tracking
//Hide/Unhide the item info fields based on the entry type selected - Barcode/Manual
function rbEntryType2Changed(value) {
   //alert(' rbEntryType2Changed called');
    if (value == 0) {

        document.getElementById("tblrow2_Barcode").style.display = 'table-row';
        document.getElementById("tblrow2_Manual1").style.display = 'none';
        document.getElementById("tblrow2_Manual2").style.display = 'none';


    }
    else {

        document.getElementById("tblrow2_Barcode").style.display = 'none';
        document.getElementById("tblrow2_Manual1").style.display = 'table-row';
        document.getElementById("tblrow2_Manual2").style.display = 'table-row';
    }
}


//Save function: set the hidden field(s) accordingly
function BarcodeStringTextbox_Changed(barcode_string) {
    //alert('in function BarcodeStringTextbox_Changed');
    document.getElementById('Track_Item_behaviors_request').value ="decode_barcode";
    document.getElementById('Track_Item_hidden_value').value = barcode_string;
  //  document.itemNavForm.submit();
    return false;
}

//function for the barcode scanned textfield on the second tab
function BarcodeStringTextbox2_Changed(barcode_string) {
    document.getElementById('Track_Item_behaviors_request').value = "decode_barcode";
    document.getElementById('Track_Item_hidden_value').value = barcode_string;
    //  document.itemNavForm.submit();
    return false;
}

//Function called when new entry is entered manually
function Add_new_entry() {
 //   alert('Function Add_new_entry called');
    document.getElementById('Track_Item_behaviors_request').value = "read_manual_entry";
    document.getElementById('hidden_BibID').value = document.getElementById('txtBibID').value;
    document.getElementById('hidden_VID').value = document.getElementById('txtVID').value;
    document.getElementById('hidden_event_num').value = document.getElementById('ddlManualEvent').value;
  
    document.itemNavForm.submit();
    return false;
}

//Function called when new entry is entered manually, through the second tab
function Add_new_entry2() {
 //   alert('function Add_new_entry2 called');
    document.getElementById('Track_Item_behaviors_request').value = "read_manual_entry";
    document.getElementById('hidden_BibID').value = document.getElementById('txtBibID2').value;
    document.getElementById('hidden_VID').value = document.getElementById('txtVID2').value;
    document.getElementById('hidden_event_num').value = document.getElementById('ddlManualEvent2').value;

    document.itemNavForm.submit();
    return false;
}

//Function called when new entry is added through a scanned barcode
function Add_new_entry_barcode()
{
    document.getElementById('Track_Item_behaviors_request').value = "decode_barcode";
    document.getElementById('Track_Item_hidden_value').value = document.getElementById('txtScannedString').value;
    document.itemNavForm.submit();
    return false;
}

//Function called when new entry is added through a scanned barcode on the second tab
function Add_new_entry_barcode2() {
   // alert('in function Add_new_entry_barcode2');
    document.getElementById('Track_Item_behaviors_request').value = "decode_barcode";
    document.getElementById('Track_Item_hidden_value').value = document.getElementById('txtScannedString2').value;
    document.itemNavForm.submit();
    return false;
}


function DisableRow_SetCSSClass(elementID) {
    
    document.getElementById(elementID).style.display = 'none';
  //  document.getElementById("tblrow_Manual1").style.display = 'none';

}

function DisableRow_RemoveCSSClass(elementID) {
  

    document.getElementById(elementID).style.display = 'table-row';


}

//Set the dropdown value for the workflow type
function SetDropdown_Selected(value_to_set) {
    if(page=='1')
        $("#ddlManualEvent").val(value_to_set);
    else {
        $("#ddlManualEvent2").val(value_to_set);
    }
}




function entry_span_mouseover(spanid) {
    $('#' + spanid).removeClass('sbkTi_TrackingEntrySpanMouseOut');
    $('#' + spanid).addClass('sbkTi_TrackingEntrySpanMouseOver');

 //   showErrorIcon(spanid);
    return false;
}

function entry_span_mouseout(spanid) {
    $('#' + spanid).removeClass('sbkTi_TrackingEntrySpanMouseOver');
    $('#' + spanid).addClass('sbkTi_TrackingEntrySpanMouseOut');

    //   showErrorIcon(spanid);
    return false;
}

//Function called when the equipment dropdownlist selected value is changed
function ddlEquipment_Changed(ddlID) {
    var ddl = document.getElementById(ddlID);
    document.getElementById('hidden_equipment').value = ddl.options[ddl.selectedIndex].value;
}

//Function called when the user dropdownlist selected value is changed
function ddlUser_Changed(ddlID) {
    var ddl = document.getElementById(ddlID);
    document.getElementById('hidden_selected_username').value = ddl.options[ddl.selectedIndex].value;
  //  alert(ddl.options[ddl.selectedIndex].value);
}

function save_item_tracking(page) {
    
    var hiddenfield = document.getElementById('tracking_new_page');
    hiddenfield.value = page;
 //   document.itemNavForm.submit();
    return false;
}



function isValidDate(dateString) {
    var result = true;
    // Checks for the following valid date formats:
    //   MM/DD/YYYY    MM-DD-YYYY
    // Also separates date into month, day, and year variables
    
    var datePat = /^(\d{1,2})(\/|-)(\d{1,2})\2(\d{4})$/;

    //Check to see if the format matches
    var matchArray = dateString.match(datePat);
    if (matchArray == null) {
        return false;
    }

    // Parse the date into variables
    var month = matchArray[1];
    var day = matchArray[3];
    var year = matchArray[4];
    // check month range
    if (month < 1 || month > 12) {
        return false;
    }

    //Check date range
    if (day < 1 || day > 31) {
        return false;
    }
    if ((month == 4 || month == 6 || month == 9 || month == 11) && day == 31) {
        return false;
    }

    // check for february 29th
    if (month == 2) {
        var isleap = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        if (day > 29 || (day == 29 && !isleap)) {
            return false;
        }
    }

    var todaysDate = new Date();

    //Check that the date is not a future date
    if (year > todaysDate.getFullYear() || (year == todaysDate.getFullYear() && month > (todaysDate.getMonth()+1) || (year == todaysDate.getFullYear() && month == todaysDate.getMonth() && day > todaysDate.getDate())))
        return false;


    return true;  // date is valid
}

//Checks that the time format is correct (hh:mm AM/PM)
function isValidTime(timeString) {
    var result = true;
 //   alert('Input time string:' + timeString);
    var timePat = /^([1-9]|[0-1]\d):([0-5]\d)\s*(AM|PM)?$/i;
    //Check to see if the format matches
    var matchArray = timeString.match(timePat);
//   alert(matchArray);
    if (matchArray == null) {
        return false;
    }
    return true;
}

//Check for a vlid Date, start time, end time combination
function isValidDateTime(start_Date, startTime, endTime) {
//    alert('in function isValidDateTime .' + start_Date+startTime+ endTime);
    var dateBeforeToday = true;

    //Convert the start and end times to the 24-hour format for easy comparison
    var timePat = /^([1-9]|[0-1]\d):([0-5]\d)\s*(AM|PM)?$/i;
    var matchArray1 = startTime.match(timePat);
    var matchArray2 = endTime.match(timePat);

    var ampm = matchArray1[3].toString().toLowerCase().trim();
    var startTime24_hours = ( ampm == 'am') ? (matchArray1[1]) : (parseInt(matchArray1[1]) + 12);
    var startTime24_min = matchArray1[2];
    var startTime24 = startTime24_hours + ":" + startTime24_min;


    if (matchArray2 != null) {
        var endTime24_hours = matchArray2[3].toString().toLowerCase().trim() == "am" ? matchArray2[1] : parseInt(matchArray2[1]) + 12;
        var endTime24_min = matchArray2[2];
        var endTime24 = endTime24_hours + ":" + endTime24_min;

    }

    var datePat = /^(\d{1,2})(\/|-)(\d{1,2})\2(\d{4})$/;

    //Check to see if the format matches
    var matchArray = start_Date.match(datePat);
    if (matchArray == null) {
        return false;
    }

    // Parse the date into variables
    var month = matchArray[1];
    var day = matchArray[3];
    var year = matchArray[4];
    
    var todaysDate = new Date();
    var currentTime24 = todaysDate.getHours() + ":" + todaysDate.getMinutes();

  //  alert(year + '  ' + todaysDate.getFullYear() + '  month: ' + month + '  ' + todaysDate.getMonth()+ ' day:' + day + ' ' + todaysDate.getDate);
    if ( year == todaysDate.getFullYear() && month == todaysDate.getMonth()+1 && day == todaysDate.getDate())
    {
        dateBeforeToday = false;
    }

    //Workflow opened before today should have both start and end times
    if (dateBeforeToday && (endTime24 == null || endTime.length == 0)) {
        alert('You must enter an end time!');
        return false;
    }

    //End time should be after start time
    if (endTime24 != null) {
  //      alert('endTime24<startTime24'+(endTime24<startTime24));
        if (endTime24 < startTime24) {
            alert('End time cannot be before Start time!');
            return false;
        }
    }
 //   alert('dateBeforeToday:' + dateBeforeToday + ',  currentTime24:'+currentTime24 + ' startTime24:' + startTime24);
    if (!(dateBeforeToday) && (startTime24 > currentTime24 || (endTime24 != null && endTime24 > currentTime24))) {
        alert('Start/End time cannot be in the future!');
        return false;
    }

    return true;
}


function save_workflow(workflow_ID, itemID) {

    //First do some validations
    if (page == 1) {
        var start_Date = document.getElementById('txtStartDate' + workflow_ID).value;
    //    alert(start_Date);
        if (start_Date == null || !isValidDate(start_Date)) {
            alert('You must enter a valid date!');
            return false;
        }
        //Check for a valid start time
        var startTime = document.getElementById('txtStartTime' + workflow_ID).value;
 //       alert(startTime);
        if (startTime == null || !isValidTime(startTime)) {
            alert('You must enter a valid start time!');
            return false;
        }
        
        //Check for a valid end time
        var endTime = document.getElementById('txtEndTime' + workflow_ID).value;
//        alert(endTime);
        if ((endTime != null && endTime.length > 0) && !isValidTime(endTime)) {
            alert('Invalid End Time entered!');
            return false;
        }

//        alert(isValidDateTime(start_Date, startTime, endTime));
        //Check for a valid date/time combination
        if (!isValidDateTime(start_Date, startTime, endTime))
            return false;
        
    }
    else if (page == 2) {
        var startDate = document.getElementById('txtStartDate2' + workflow_ID).value;
   //        alert(startDate.value);
        if (startDate == null || !isValidDate(startDate)) {
            alert('You must enter a valid date!');
            return false;
        }
    }

 
     //If all the fields have valid inputs, go ahead and set the hidden variables to save this entry
        document.getElementById('Track_Item_behaviors_request').value = 'save';
        document.getElementById('Track_Item_hidden_value').value = workflow_ID;
        document.getElementById('hidden_itemID').value = itemID;
  //      alert(document.getElementById('Track_Item_behaviors_request').value);
   //     alert(document.getElementById('Track_Item_hidden_value').value);
        document.itemNavForm.submit();
        
 
    return true;
}





function delete_workflow(workflow_ID) {
    //alert(workflow_ID);
    document.getElementById('Track_Item_behaviors_request').value = 'delete';
    document.getElementById('Track_Item_hidden_value').value = workflow_ID;

    document.itemNavForm.submit();
    return false;
}



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

            //store which tab we are on
            //        var navitem = container.querySelector("li");
            //     var ident = navitem.id.split("_")[1];
        
              var  navitem = document.getElementById('tabHeader_'+page);
               var ident = page;
            

            navitem.parentNode.setAttribute("data-current", ident);
            
 
            //set current tab with class of activetabheader
            navitem.setAttribute("class", "tabActiveHeader");

            //hide the tab contents we don't need
            var pages = container.querySelectorAll(".tabpage");
            for (var i = 0; i < pages.length; i++) {
                if (i == (page - 1))
                    continue;
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
            
            //Set the hidden value to indicate the tab selected
             save_item_tracking(ident);

        }
    };
})(jQuery);

function setDatePicker(elementID) {

    var thisElement = document.getElementById(elementID);
 //   alert(thisElement);
    $( "#"+elementID ).datepicker();
}

function setTimePicker(elementID) {

    var thisElement = document.getElementById(elementID);
 //     alert(thisElement);
    $("#" + elementID).timeEntry();
    
    //Also apply the formatter.js formatter to auto-format user entered time
   // alert('Applying formatter to '+elementID);
    //$("#" + elementID).formatter({
    //    'pattern' :  '{{99}}:{{99}} {{a}}{{M}}',
    //'persistent' : true});
    
    //$('#'+elementID).formatter({
    //    'pattern': '{{999}}-{{999}}-{{999}}-{{9999}}',
    //    'persistent': true
    //});
}