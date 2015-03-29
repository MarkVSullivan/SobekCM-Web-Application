/*! Copyright (c) 2013 Brandon Aaron (http://brandonaaron.net)
 * Licensed under the MIT License (LICENSE.txt).
 *
 * Thanks to: http://adomas.org/javascript-mouse-wheel/ for some pointers.
 * Thanks to: Mathias Bank(http://www.mathias-bank.de) for a scope bug fix.
 * Thanks to: Seamus Leahy for adding deltaX and deltaY
 *
 * Version: 3.1.3
 *
 * Requires: 1.2.2+
 */
(function(n){typeof define=="function"&&define.amd?define(["jquery"],n):typeof exports=="object"?module.exports=n:n(jQuery)})(function(n){function f(i){var u=i||window.event,l=[].slice.call(arguments,1),f=0,e=0,o=0,h=0,c=0,s;return i=n.event.fix(u),i.type="mousewheel",u.wheelDelta&&(f=u.wheelDelta),u.detail&&(f=u.detail*-1),u.deltaY&&(o=u.deltaY*-1,f=o),u.deltaX&&(e=u.deltaX,f=e*-1),u.wheelDeltaY!==undefined&&(o=u.wheelDeltaY),u.wheelDeltaX!==undefined&&(e=u.wheelDeltaX*-1),h=Math.abs(f),(!r||h<r)&&(r=h),c=Math.max(Math.abs(o),Math.abs(e)),(!t||c<t)&&(t=c),s=f>0?"floor":"ceil",f=Math[s](f/r),e=Math[s](e/t),o=Math[s](o/t),l.unshift(i,f,e,o),(n.event.dispatch||n.event.handle).apply(this,l)}var e=["wheel","mousewheel","DOMMouseScroll","MozMousePixelScroll"],i="onwheel"in document||document.documentMode>=9?["wheel"]:["mousewheel","DomMouseScroll","MozMousePixelScroll"],r,t,u;if(n.event.fixHooks)for(u=e.length;u;)n.event.fixHooks[e[--u]]=n.event.mouseHooks;n.event.special.mousewheel={setup:function(){if(this.addEventListener)for(var n=i.length;n;)this.addEventListener(i[--n],f,!1);else this.onmousewheel=f},teardown:function(){if(this.removeEventListener)for(var n=i.length;n;)this.removeEventListener(i[--n],f,!1);else this.onmousewheel=null}};n.fn.extend({mousewheel:function(n){return n?this.bind("mousewheel",n):this.trigger("mousewheel")},unmousewheel:function(n){return this.unbind("mousewheel",n)}})})