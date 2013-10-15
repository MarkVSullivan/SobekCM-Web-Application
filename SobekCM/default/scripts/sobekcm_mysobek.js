
// Delete an item
function delete_item() {
	var hiddenfield = document.getElementById('admin_delete_item');
	hiddenfield.value = "delete";
	document.itemNavForm.submit();
	return false;
}


function close_mysobek_form(FormName) {
	// Close the associated form
	popdown(FormName);

	// Return TRUE to cause a return trip to the server
	return true;
}

// Populate the project form and show it
function popup_mysobek_form(FormName, FocusName) {

	// Toggle the form
	blanket_size(FormName, 215);
	toggle('blanket_outer');
	toggle(FormName);

	// Create the draggable object to allow this window to be dragged around
	$("#" + FormName).draggable();

	// Put focus on the focus element
	if (FocusName.length > 0) {
		var focusfield = document.getElementById(FocusName);
		focusfield.focus();
	}

	return false;
}

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