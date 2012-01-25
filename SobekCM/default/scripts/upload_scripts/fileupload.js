/*

File Upload HTTP module for ASP.Net (v 2.0)
Copyright (C) 2007-2008 Darren Johnstone (http://darrenjohnstone.net)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

*/

var up_xhttp;
var up_key;
var up_loading = false;
var up_imagePath;

function up_AddUpload(id)
{
    var els = document.getElementById(id).getElementsByTagName('div');
    
    for (var i=0; i<els.length; i++)
    {
        if (els[i].className == 'upFileInputs upHiddenDynamic')
        {
            els[i].className = 'upFileInputs';
            break;
        }
        
        if (els[i].className == 'upContainerNormal upHiddenDynamic')
        {
            els[i].className = 'upContainerNormal';
            break;
        }
    }
}

function up_ValidateUpload(source, clientside_arguments)
{        
    var els = source.parentNode.getElementsByTagName('input');
    var ok = false;
    
    for (var i=0; i<els.length; i++)
    {
        if (els[i].type == 'file')
        {
            if (els[i].value !=  null && els[i].value != '')
            {
                ok = true;
                break;
            }
        }
    }    
    clientside_arguments.IsValid = ok;
}

function up_ValidateUploadExtensions(source, clientside_arguments)
{        
    var els = source.parentNode.getElementsByTagName('input');
    var extField = source.previousSibling;
    
    var ok = true;
    if (extField.value != null && extField.value != '')
    {
        var exts = extField.value.split(',');
        
        for (var i=0; i<els.length; i++)
        {
            if (els[i].type == 'file')
            {
                if (els[i].value != null && els[i].value != '')
                {
                    var valid = false;
                    for (var j=0; j<exts.length; j++)
                    {
                        if (els[i].value.toLowerCase().lastIndexOf(exts[j]) != -1)
                        {
                            valid = true;
                            break;
                        }
                    }
                    
                    ok = ok && valid;
                    if (!ok) break;
                }
            }
        }    
    }
    clientside_arguments.IsValid = ok;
}

function up_ValidateHaveUploads()
{        
    var els = document.getElementsByTagName('input');
    
    for (var i=0; i<els.length; i++)
    {
        if (els[i].type == 'file')
        {
            if (els[i].value != null && els[i].value != '')
            {
                return true;
            }
        }
    }
    
    return false;
}

function up_ClearUploadElement(el)
{
    var newItem = document.createElement('input');
    newItem.type = 'file'
    newItem.id = el.id;
    newItem.name = el.name;
    newItem.className = el.className;
    newItem.relatedElement = el.relatedElement;
    newItem.onchange = el.onchange;
    newItem.onmouseout = el.onmouseout;
    
    var parent = el.parentNode;
    parent.replaceChild(newItem, el);
    
    parent.className += ' upHiddenDynamic';
}

function up_RemoveUpload(e)
{
    var container = e.parentNode;
    var els = container.getElementsByTagName('input');
    
    for (var i=0; i<els.length; i++)
    {
        if (els[i].type == 'text')
        {
            els[i].value = '';
        }
            
        if (els[i].type == 'file')
        {
            up_ClearUploadElement(els[i]);
        }
    }    
}

function up_BeginUpload(key, showcancel, url)
{
    if (!Page_IsValid) return;
    if (navigator.userAgent.indexOf("Opera") != -1) return;
    if (document.getElementById(key) == null) return; 
    if (!up_ValidateHaveUploads()) return;
    
    up_key = document.getElementById(key).value;
    up_showCancel = showcancel;
    up_SetProgress(0, '');
    window.setInterval('up_ReportProgress()', 1500);
}

function up_SetProgress(progress, file)
{
    var html;
    html  = "<div class='upContainer'>";
    html += "<div class='upOuterBar'>";
    html += "<div id='upProgressBar' class='upInnerBar' style='width:" + progress + "%'>";
    html += "</div>";
    html += "<div id='upLabel' class='upLabel'>";
    html += (progress == 0 ? "Waiting for uploads to start" : "Now uploading " + file + " " + progress + "%");
    html += "</div>";
    html += "</div>";
    html += "</div>";
    
    // Cancel button
    if (up_showCancel)
    {
        html += "<img src='" + up_imagePath + "cancelbutton.gif' onclick='document.location=document.location' style='cursor:hand;margin-top:5px' />";
    }

    if (!up_loading)
    {
        Modalbox.show(html, {title: "Upload in progress", width: 600, height: 120, aspnet:true});
        Modalbox.deactivate();
    }
    else
    {
        $(Modalbox.MBcontent).update(html);
    }
    
    up_loading = true;
}

var up_waiting = false;

function up_ReportProgress()
{  
   if (up_waiting) return;
   up_waiting = true;
   
    if (typeof(XMLHttpRequest) != 'undefined')
    {
	    up_xhttp = new XMLHttpRequest();
    }
    else if (typeof(ActiveXObject) != 'undefined')
    {
	    up_xhttp = new ActiveXObject('Microsoft.XMLHTTP');
    }
    else return;

    var ts = new Date().getTime(); // Time stamp to stop caching
    up_xhttp.open('GET', 'UploadProgress.ashx?DJUploadStatus=' + up_key + '&ts=' + ts, false); 
    up_xhttp.send('');
    
    if (up_xhttp.status == 200)
    {
        var res = up_xhttp.responseXML;
        
        if (res.documentElement.getAttribute('badlength') == 'true')
        {
            alert('The maximum file upload size was exceeded. The upload will abort.');
            document.location=document.location;
        }
        
        if (res.documentElement.getAttribute('empty') == 'true')
        {
            up_SetProgress(0, '');
        }
        else
        {
            var progress = parseInt(res.documentElement.getAttribute('progress'));
            var file = res.documentElement.getAttribute('file');
            up_SetProgress(progress, file);
        }
    }
    else
    {
        up_SetProgress(0, up_xhttp.statusText);
    }
	up_waiting = false;
}

function up_createDynamicStyles(cssPath)
{
    var cssNode = document.createElement('link');
    cssNode.setAttribute('rel', 'stylesheet');
    cssNode.setAttribute('type', 'text/css');
    cssNode.setAttribute('href', cssPath + 'dynamicstyles.css');
    document.getElementsByTagName('head')[0].appendChild(cssNode); 
}

function up_killProgress(id)
{
    if (navigator.userAgent.indexOf("Opera") != -1) return;
    
    var el = document.getElementById(id);
    el.parentNode.removeChild(el);
}


/* ======================================================================
The following function for styling input controls comes from
http://www.quirksmode.org/dom/inputfile.html
========================================================================= */

function up_initFileUploads(imagepath) 
{
    up_imagePath = imagepath;

	// Bug fix: show cancel button in ModalBox in safari
	var preload_cancel = new Image();
	preload_cancel.src = up_imagePath + 'cancelbutton.gif';
    
    var W3CDOM = (document.createElement && document.getElementsByTagName);
    if (!W3CDOM) return;
    var fakeFileUpload = document.createElement('div');
    fakeFileUpload.className = 'upFakeFile';
    var txt = document.createElement('input');
    txt.className = 'upFileBox';
    fakeFileUpload.appendChild(txt);
    var image = document.createElement('img');
    image.src= up_imagePath + 'selectbutton.gif';
    image.style.cursor = 'hand';
    image.style.marginLeft = '5px';
    image.className = 'upSelectButton';
    fakeFileUpload.appendChild(image);
    var x = document.getElementsByTagName('input');
    for (var i=0;i<x.length;i++) {
	    if (x[i].type != 'file') continue;
	    if (x[i].parentNode.className.indexOf('upFileInputs') != 0  || x[i].relatedElement != null) continue;

	    x[i].className = 'upFile hidden';
	    var clone = fakeFileUpload.cloneNode(true);
	    x[i].parentNode.appendChild(clone);
	    x[i].relatedElement = clone.getElementsByTagName('input')[0];
	    x[i].onchange = x[i].onmouseout = function () 
	    {
	        if (this.cleared || this.value == '') return;
	        
            var val = this.value;
		    val = val.substr(val.lastIndexOf('\\') + 1, val.length);
		    val = val.substr(val.lastIndexOf('/') + 1, val.length);
		    this.relatedElement.value = val;
	    }
    }
}