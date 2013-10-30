
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
    document.getElementById('Track_Item_behaviors_request').value ="decode_barcode";
    document.getElementById('Track_Item_hidden_value').value = barcode_string;
    document.itemNavForm.submit();
    return false;
}