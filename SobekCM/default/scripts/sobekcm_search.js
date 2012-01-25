// This contains all the basic search javascript which takes the user's entries ( client-side )
// and converts to the search URL and requests the appropriate results to match the search

function fnTrapKD(event, type, arg1, arg2, browseurl ) {

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

        if (arg2.length == 0) {
            if (type == 'basic')
                basic_search_sobekcm(arg1, browseurl);
            if (type == 'metadata')
                metadata_search_sobekcm(arg1, browseurl);
            if (type == 'newspaper')
                newspaper_search_sobekcm(arg1, browseurl);
            if (type == 'dloc')
                dloc_search_sobekcm(arg1, browseurl);
            if (type == 'text')
                fulltext_search_sobekcm(arg1, browseurl);
        }
        else {
            if ( type == 'basic' )
                basic_select_search_sobekcm(arg1, arg2);
            if (type == 'metadata')
                metadata_select_search_sobekcm(arg1, arg2);
            if (type == 'newspaper')
                newspaper_select_search_sobekcm(arg1, arg2);
            if (type == 'dloc')
                dloc_select_search_sobekcm(arg1, arg2);
            if (type == 'text')
                fulltext_select_search_sobekcm(arg1, arg2);
        }

        return false;
    }
}


// Advanced search
function advanced_search_sobekcm( root )
{
	// Collect and trim the users's search string
    var term = trimString(document.search_form.Textbox1.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox2.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox3.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox4.value).replace(",", "+").replace(" ", "+");
	var fields = document.search_form.Dropdownlist1.value + "," + document.search_form.andOrNotBox1.value + document.search_form.Dropdownlist2.value + "," + document.search_form.andOrNotBox2.value + document.search_form.Dropdownlist3.value + "," + document.search_form.andOrNotBox3.value + document.search_form.Dropdownlist4.value;
	
	if ( term.length > 0 )
	{				
		// Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";
        
		// replace ' or ' and ' and ' and ' not ' in the query
        // STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        //term = term.toLowerCase().replace(" or "," =").replace(" and ", " ").replace(" not "," -");

        // Determine if a precision was selected
        var contains_radio = document.getElementById('precisionContains');
        if (( contains_radio != null ) && ( contains_radio.checked == true))
             root = root.replace("results", contains_radio.value);

        var results_radio = document.getElementById('precisionResults');
        if (( results_radio != null ) && ( results_radio.checked == true))
            root = root.replace("results", results_radio.value);
             
        var like_radio = document.getElementById('precisionLike');
        if (( like_radio != null ) && ( like_radio.checked == true))
             root = root.replace("results", like_radio.value);

        // Build the destination url by placing the selection option first
        var url = root + "?t=" + term + "&f=" + fields;
        if (root.indexOf("?") > 0)
            url = root + "&t=" + term + "&f=" + fields;
		
		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Advanced search in an aggregation with children selectable
function advanced_select_search_sobekcm( root, next_level )
{
	// Collect and trim the users's search string
    var term = trimString(document.search_form.Textbox1.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox2.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox3.value).replace(",", "+").replace(" ", "+") + "," + trimString(document.search_form.Textbox4.value).replace(",", "+").replace(" ", "+");
    var fields = document.search_form.Dropdownlist1.value + "," + document.search_form.andOrNotBox1.value + document.search_form.Dropdownlist2.value + "," + document.search_form.andOrNotBox2.value + document.search_form.Dropdownlist3.value + "," + document.search_form.andOrNotBox3.value + document.search_form.Dropdownlist4.value;
	
	if ( term.length > 0 )
	{		
		// Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";
        		
		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");
		
		// Build the destination url by placing the selection option first
        var url = root + "?t=" + term + "&f=" + fields;
        if (root.indexOf("?") > 0)
            url = root + "&t=" + term + "&f=" + fields;
		
		// Do additional work if there are checkboxes on this form
		if ( document.search_form.checkgroup.length > 0 )
		{
			// Check to see if any of the checkboxes are NOT checked
			var test=true;
			for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
			{
				if (document.search_form.checkgroup[i].checked==false)
				{
					test=false;
				}
			}
			
			// If some were unchecked, include the codes for the checked boxes
			var found = 0;
			if ( test == false )
			{
				if ( next_level.indexOf("s") >= 0 )
				{
					url = url + "&" + next_level + "-";
					
					for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
					{
						if (document.search_form.checkgroup[i].checked==false)
						{
							if ( found > 0 )
							{
								url = url + "," + document.search_form.checkgroup[i].value ;
							}
							else
							{
								url = url + document.search_form.checkgroup[i].value;
							}
							found++;
						}
					}
				}
				else
				{
					url = url + "&" + next_level + ".";
					
					for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
					{
						if (document.search_form.checkgroup[i].checked==true)
						{
							url = url + document.search_form.checkgroup[i].value + ",";
						}
					}		
				}
			}
		}
		
		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Full text
function fulltext_search_sobekcm(root, browseurl) {
    // Collect and trim the users's search string
    var term = trimString(document.search_form.u_search.value).replace(",", " ");
    if ((term.length > 0) && (term != '*')) {
        // Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";

        // replace ' or ' and ' and ' and ' not ' in the query
        // STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -"); ;

        // Build the destination url by placing the selection option first and redirect
        var url = root + "?text=" + term;
        if (root.indexOf("?") > 0)
            url = root + "&text=" + term;
        window.location.href = url;
    }
    else if (browseurl.length > 0) {
        window.location.href = browseurl;
    }
}

// Basic search in an aggregation with children selectable
function fulltext_select_search_sobekcm(root, next_level) {
    // Collect and trim the users's search string
    var term = trimString(document.search_form.u_search.value).replace(",", " ");
    if (term.length > 0) {
        // Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";

        // replace ' or ' and ' and ' and ' not ' in the query
        // STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -"); ;

        // Build the destination url by placing the selection option first
        var url = root + "?text=" + term;
        if (root.indexOf("?") > 0)
            url = root + "&text=" + term;

        // Do additional work if there are checkboxes on this form
        if (document.search_form.checkgroup.length > 0) {
            // Check to see if any of the checkboxes are NOT checked
            var test = "true";
            for (i = 0; i < document.search_form.checkgroup.length; i++) {
                if (document.search_form.checkgroup[i].checked == false) {
                    test = "false";
                }
            }

            // If some were unchecked, include the codes for the checked boxes
            if (test == "false") {
                url = url + "&" + next_level + ".";

                for (i = 0; i < document.search_form.checkgroup.length; i++) {
                    if (document.search_form.checkgroup[i].checked == true) {
                        url = url + document.search_form.checkgroup[i].value + ",";
                    }
                }
            }
        }


        // Change the the browser location to the new url
        window.location.href = url.replace('m=hrb', 'm=hrd');
    }
}

// Basic search
function basic_search_sobekcm(root, browseurl)
{   
	// Collect and trim the users's search string
    var term = trimString(document.search_form.u_search.value).replace(",", " ");
    if ((term.length > 0) && (term != '*')) {

	    if (term.toLowerCase() == "floridaxx") {
	        alert('Please narrow your search by entering additional search terms.');
	    }
	    else {
	        // Show the progress spinner
	        var circular_div = document.getElementById("circular_progress");
	        if (circular_div != null) {
	            circular_div.className = "shown_progress_gray";
	        }

	        // replace ' or ' and ' and ' and ' not ' in the query
	        // STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
	        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

	        // Build the destination url by placing the selection option first and redirect
	        var url = root + "?t=" + term;
	        if (root.indexOf("?") > 0)
	            url = root + "&t=" + term
	            
	        window.location.href = url;
	    }
	}
	else if (browseurl.length > 0) {
	    window.location.href = browseurl;
	}
}

// Basic search in an aggregation with children selectable
function basic_select_search_sobekcm( root, next_level )
{    
	// Collect and trim the users's search string
    var term = trimString(document.search_form.u_search.value).replace(",", " ");
	if ( term.length > 0 )
	{				
	    // Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";
    
		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

        // Build the destination url by placing the selection option first
        var url = root + "?t=" + term;
        if (root.indexOf("?") > 0)
            url = root + "&t=" + term
		
		// Do additional work if there are checkboxes on this form
		if ( document.search_form.checkgroup.length > 0 )
		{
			// Check to see if any of the checkboxes are NOT checked
			var test="true";
			for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
			{
				if (document.search_form.checkgroup[i].checked==false)
				{
					test="false";
				}
			}
			
			// If some were unchecked, include the codes for the checked boxes
			if ( test == "false" )
			{
				url = url + "&" + next_level + ".";
				
				for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
				{
					if (document.search_form.checkgroup[i].checked==true)
					{
						url = url + document.search_form.checkgroup[i].value + ",";
					}
				}		
			}
		}


		// Change the the browser location to the new url
	    window.location.href = url.replace('m=hrb','m=hrd');
	}
}

// dLOC Search
function dloc_search_sobekcm(root, browseurl) {
    // Collect and trim the users's search string
    var term = trimString(document.search_form.u_search.value).replace(",", " ");
    if ((term.length > 0) && (term != '*')) {
        // Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";

        // replace ' or ' and ' and ' and ' not ' in the query
        // STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

        // determine the base url
        var url = root + "?t=" + term;
        if (root.indexOf("?") > 0)
            url = root + "&t=" + term;

        // Build the destination url by placing the selection option first and redirect
        if (document.search_form.newscheck.checked == false) {
            url = url + ",newspaper&f=TX,-FC";
            window.location.href = url;
        }
        else {
            window.location.href = url;
        }
    }
    else if (browseurl.length > 0) {
        window.location.href = browseurl;
    }
}

// dLOC search in an aggregation with children selectable
function dloc_select_search_sobekcm( root, next_level )
{
	// Collect and trim the users's search string
    var term = trimString(document.search_form.u_search.value).replace(",", " ");
	if ( term.length > 0 )
	{				
		// Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";
        
		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");
		
        // determine the base url
        var url = root + "?t=" + term;
        if (root.indexOf("?") > 0)
            url = root + "&t=" + term;
		
		// Do additional work if there are checkboxes on this form
		if ( document.search_form.checkgroup.length > 0 )
		{
			// Check to see if any of the checkboxes are NOT checked
			var test="true";
			for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
			{
				if (document.search_form.checkgroup[i].checked==false)
				{
					test="false";
				}
			}
			
			// If some were unchecked, include the codes for the checked boxes
			if ( test == "false" )
			{
				url = url + "&" + next_level + ".";
				
				for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
				{
					if (document.search_form.checkgroup[i].checked==true)
					{
						url = url + document.search_form.checkgroup[i].value + ",";
					}
				}		
			}
		}
		
		
				// Build the destination url by placing the selection option first and redirect
		if ( document.search_form.newscheck.checked == false )
		{
		    if (document.search_form.textcheck.checked == false)
	    	{
    	        window.location.href = url.replace('m=hrb','m=hra').replace('m=lrb','m=lra');
    		}
	    	else
	        {
    		    window.location.href = url.replace('m=hrb','m=hra').replace('m=lrb','m=lra');
	        }		
		}
		else
		{
    		if (document.search_form.textcheck.checked == false)
	    	{
		        window.location.href = url.replace('m=hrb','m=hrd').replace('m=lrb','m=lrd');
    		}
	    	else
	        {
    		    window.location.href = url;
	        }
	    }
	}
}

// Newspaper search
function newspaper_search_sobekcm(root, browseurl)
{
	// Collect and trim the users's search string
	if ( document.search_form.Dropdownlist1.value == "PP" )
	{
	    var term = trimString(document.search_form.Textbox1.value).replace(",", " ") + "," + trimString(document.search_form.Textbox1.value).replace(",", " ");
	    var fields = "PP,=SP";
	}
	else
	{
	    var term = trimString(document.search_form.Textbox1.value).replace(",", " ");
	    var fields = document.search_form.Dropdownlist1.value;
	}

	if ((term.length > 0) && (term != '*')) {
	    // Show the progress spinner
	    var circular_div = document.getElementById("circular_progress");
	    circular_div.className = "shown_progress_gray";

	    // replace ' or ' and ' and ' and ' not ' in the query
	    // STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
	    term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

	    var url = root + "?t=" + term + "&f=" + fields;
	    if (root.indexOf("?") > 0)
	        url = root + "&t=" + term + "&f=" + fields;

	    // Change the the browser location to the new url
	    window.location.href = url;
	}
	else if (browseurl.length > 0) {
	    window.location.href = browseurl;
	}
}

// Newspaper search in an aggregation with children selectable
function newspaper_select_search_sobekcm( root, next_level )
{
	// Collect and trim the users's search string
	if ( document.search_form.Dropdownlist1.value == "PP" )
	{
	    var term = trimString(document.search_form.Textbox1.value).replace(",", " ") + "," + trimString(document.search_form.Textbox1.value).replace(",", " ");
	    var fields = "PP,=SP";
	}
	else
	{
	    var term = trimString(document.search_form.Textbox1.value).replace(",", " ");
	    var fields = document.search_form.Dropdownlist1.value;
	}
	
	if ( term.length > 0 )
	{				
		// Show the progress spinner
        var circular_div = document.getElementById("circular_progress");
        circular_div.className = "shown_progress_gray";
        
		// replace ' or ' and ' and ' and ' not ' in the query
		// STILL NEED TO REPLACE THESE FOR FRENCH AND SPANISH
        term = term.toLowerCase().replace(" or ", " =").replace(" and ", " ").replace(" not ", " -").replace(" y no ", " -").replace(" y ", " =").replace(" o ", " ").replace(" no ", " -");

        // URL
        var url = root + "?t=" + term + "&f=" + fields;
        if (root.indexOf("?") > 0)
            url = root + "&t=" + term + "&f=" + fields;
		
		// Do additional work if there are checkboxes on this form
		if ( document.search_form.checkgroup.length > 0 )
		{
			// Check to see if any of the checkboxes are NOT checked
			var test=true;
			for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
			{
				if (document.search_form.checkgroup[i].checked==false)
				{
					test=false;
				}
			}
			
			// If some were unchecked, include the codes for the checked boxes
			var found = 0;
			if ( test == false )
			{
				if ( next_level.indexOf("s") >= 0 )
				{
					url = url + "&" + next_level + "-";
					
					for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
					{
						if (document.search_form.checkgroup[i].checked==false)
						{
							if ( found > 0 )
							{
								url = url + "," + document.search_form.checkgroup[i].value ;
							}
							else
							{
								url = url + document.search_form.checkgroup[i].value;
							}
							found++;
						}
					}
				}
				else
				{
					url = url + "&" + next_level + ".";
					
					for ( i=0 ; i < document.search_form.checkgroup.length ; i++)
					{
						if (document.search_form.checkgroup[i].checked==true)
						{
							url = url + document.search_form.checkgroup[i].value + ",";
						}
					}		
				}
			}
		}
		
		// Change the the browser location to the new url
		window.location.href = url;
	}
}

// Trim the input string from the search box
function trimString (str) 
{
  str = this != window? this : str;
  return str.replace(/^\s+/g, '').replace(/\s+$/g, '');
}

