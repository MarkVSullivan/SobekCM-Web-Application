//Global variables
var page = 1;



//Set the global page value: this indicates the current tab selected
function setCurrentTab(thisPage) {
   
    page = thisPage;
    alert(page);
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
    alert(' rbEntryType2Changed called');
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

    document.getElementById('Track_Item_behaviors_request').value ="decode_barcode";
    document.getElementById('Track_Item_hidden_value').value = barcode_string;
    document.itemNavForm.submit();
    return false;
}

//Function called when new entry is entered manually
function Add_new_entry() {
  
    document.getElementById('Track_Item_behaviors_request').value = "read_manual_entry";
    document.getElementById('hidden_BibID').value = document.getElementById('txtBibID').value;
    document.getElementById('hidden_VID').value = document.getElementById('txtVID').value;
    document.getElementById('hidden_event_num').value = document.getElementById('ddlManualEvent').value;
  
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


function DisableRow_SetCSSClass(elementID) {
    
    document.getElementById(elementID).style.display = 'none';
  //  document.getElementById("tblrow_Manual1").style.display = 'none';

}

function DisableRow_RemoveCSSClass(elementID) {
  

    document.getElementById(elementID).style.display = 'table-row';


}

//Set the dropdown value for the workflow type
function SetDropdown_Selected(value_to_set) {
    $("#ddlManualEvent").val(value_to_set);
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
    document.getElementById('hidden_selected_user').value = ddl.options[ddl.selectedIndex].value;
  //  alert(ddl.options[ddl.selectedIndex].value);
}

function save_item_tracking(page) {
    
    var hiddenfield = document.getElementById('tracking_new_page');
    hiddenfield.value = page;
 //   document.itemNavForm.submit();
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