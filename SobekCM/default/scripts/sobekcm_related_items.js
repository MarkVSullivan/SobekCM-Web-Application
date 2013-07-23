
//Set the width of the thumbnail and its parent span
function SetParentSpanWidth(imageID, parentID)
{
	$=document;
	$parent=$.getElementById(parentID);
	$child = $.getElementById(imageID);
    $child.style.width = $child.offsetWidth + 'px';
	$child.style.display = 'block';

    $parent.style.width = ($child.offsetWidth + 5) + 'px';
	$parent.style.overflow = 'hidden';
	$parent.style.display = 'inline-block';

}
function SetSelectedIndexNT()
{
alert(ntIndex);
document.getElementById('selectNumOfThumbnails').options[ntIndex].selected='true';
}

//Make the span change color and fadeout over 4 seconds
function AddAnchorDivEffect(urlWithAnchor)
{
    var spanID = 'span' + urlWithAnchor.substring(urlWithAnchor.indexOf('#') + 1);
    
    var color = $('.thumbnailspan').css('backgroundColor');


    
	//document.getElementById(spanID).style.background="#FFFF00";
			document.getElementById(spanID).className='fadeEffect';
	
			$(".fadeEffect").animate({ backgroundColor: color }, 4000,
			    function () {
                    document.getElementById(spanID).className = 'thumbnailspan';
			        alert(spanID);
			});

	
}

//Make the appropriate span change color on pageload, if anchor present in the url
function MakeSpanFlashOnPageLoad()
{
		
    //   //Make page numbers at the bottom of the screen visible only if there is a scrollbar
	//	// Check if body height is higher than window height 
	//	if ($("body").height() > $(window).height()) { 

	//	//   var pageSpanID=document.getElementById('pageNumbersBottom');
	//	 //  pageSpanID.style.visibility='visible'; 
		
	//	} 
	//	else
	//	{
	//	// var pageSpanID=document.getElementById('pageNumbersBottom');
	//	// pageSpanID.style.visibility='hidden'; 
		
	//	}


    ////Change span color and fadeout over 4 seconds
    //if (window.location.hash.length > 0) {
    //    var spanID = 'span' + window.location.hash.substring(1);

    //    var color = $('.thumbnailspan').css('backgroundColor');
    //    document.getElementById(spanID).className = 'fadeEffect';        
       

    //    $(".fadeEffect").animate({ backgroundColor: color }, 4000, function() { document.getElementById(spanID).className = 'thumbnailspan'; });
            
    //    //set the selected value of the "Go to thumbnail:" dropdown list
    //    $('#selectGoToThumbnail').val(window.location.href);
    //}
}




//Add a search parameter to a url querystring if not already present
//or update it to the value passed-in if present
function UpdateQueryString(key, value, url) 
{
    if (!url) 
	    url = window.location.href;
    var re = new RegExp("([?|&])" + key + "=.*?(&|#|$)", "gi");

    if (url.match(re)) 
	{
        if (value)
            return url.replace(re, '$1' + key + "=" + value + '$2');
        else
            return url.replace(re, '$2');
    }
    else 
	{
        if (value) 
		{
            var separator = url.indexOf('?') !== -1 ? '&' : '?',
                hash = url.split('#');
            url = hash[0] + separator + key + '=' + value;
            if (hash[1]) url += '#' + hash[1];
            return url;
        }
        else
            return url;
    }
}

//Check if window has a vertical scrollbar
function HasVerticalScroll()
{

	// Check if body height is higher than window height :) 
	if ($("body").height() > $(window).height()) { 
	alert("scrollbar present");
	   document.getElementById(pageNumbersBottom).style.visibility='visible'; 
		alert("Vertical Scrollbar!"); 
	} 
	else
	{
	alert("scrollbar not present");
	document.getElementById(pageNumbersBottom).style.visibility='hidden'; 
	}

}


//On Window resize, make the page numbers & buttons at the bottom visible(or not) 
//based on the window size
function WindowResizeActions()
{
//Make page numbers at the bottom of the screen visible only if there is a scrollbar
        $(document).ready(function(){
		    $(window).resize(function(){
		                // Check if body height is higher than window height 
					if ($("body").height() > $(window).height()) { 
					  // alert("Window resized!"); 
					   var pageSpanID=document.getElementById('pageNumbersBottom');
					   pageSpanID.style.visibility='visible'; 
 
					} 
					else
					{
					 var pageSpanID=document.getElementById('pageNumbersBottom');
					 pageSpanID.style.visibility='hidden'; 
				
					}
						});
				})  ;

		
}


