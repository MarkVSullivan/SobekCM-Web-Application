
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

//Save function: set the hidden field(s) accordingly
function BarcodeStringTextbox_Changed(barcode_string) {
    //  alert(barcode_string);
//    document.getElementById('TI_entry_type').value = "barcode";
    document.getElementById('Track_Item_behaviors_request').value ="decode_barcode";
    document.getElementById('Track_Item_hidden_value').value = barcode_string;
    document.itemNavForm.submit();
    return false;
}

function Add_new_entry() {
  //  document.getElementById('TI_entry_type').value = "manual";
    document.getElementById('Track_Item_behaviors_request').value = "read_manual_entry";
    document.getElementById('hidden_BibID').value = document.getElementById('txtBibID').value;
    document.getElementById('hidden_VID').value = document.getElementById('txtVID').value;
    document.getElementById('hidden_event_num').value = document.getElementById('ddlManualEvent').value;
    alert('Add button clicked');
    document.itemNavForm.submit();
    return false;
}