
//Hide/Unhide the item info fields based on the entry type selected - Barcode/Manual
function rbEntryTypeChanged(value) {
    if (value == 0) {
        
        document.getElementById("tblrow_Barcode").style.display = 'table-row';
        document.getElementById("tblrow_Manual1").style.display = 'none';
        document.getElementById("tblrow_Manual2").style.display = 'none';
        
        ////Disable all textboxes and dropdowns within the non-selected entry type(manual)
        //$("#tblrow_Barcode").find('input,select').attr('disabled', false);
        //$("#tblrow_Manual1").find('input,select').attr('disabled', 'disabled');
        //$("#tblrow_Manual2").find('input,select').attr('disabled', 'disabled');
        
        ////Also apply the appropriate CSS class to the table rows 
        //document.getElementById("tblrow_Barcode").className = "";
        //document.getElementById("tblrow_Manual1").className = "sbkTi_tblRowDisabled";
        //document.getElementById("tblrow_Manual2").className = "sbkTi_tblRowDisabled";

    }
    else
    {
        ////Disable all textboxes and dropdowns within the non-selected entry type(barcode)
        //$("#tblrow_Barcode").find('input,select').attr('disabled', 'disabled');
        //$("#tblrow_Manual1").find('input,select').attr('disabled', false);
        //$("#tblrow_Manual2").find('input,select').attr('disabled', false);
        
        ////Also apply the appropriate CSS class to the table rows 
        //document.getElementById("tblrow_Barcode").className = "sbkTi_tblRowDisabled";
        //document.getElementById("tblrow_Manual1").className = "";
        //document.getElementById("tblrow_Manual2").className = "";
        
        document.getElementById("tblrow_Barcode").style.display = 'none';
        document.getElementById("tblrow_Manual1").style.display = 'table-row';
        document.getElementById("tblrow_Manual2").style.display = 'table-row';
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
    
    //Enable all textboxes and dropdowns within the selected entry type row(manual)
  //  $("#"+elementID).find('input,select').attr('disabled', 'disabled');
    document.getElementById(elementID).style.display = 'none';
  //  document.getElementById("tblrow_Manual1").style.display = 'none';

}

function DisableRow_RemoveCSSClass(elementID) {
    
    //Enable all textboxes and dropdowns within the selected entry type row(barcode)
 //   $("#" + elementID).find('input,select').attr('disabled', false);
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