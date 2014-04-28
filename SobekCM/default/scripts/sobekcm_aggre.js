//$(document).ready(function() {
    
//    $('[id*=sbkAghsw_CollectionButtonImg').each(function() {
//        var $this = $(this);


//     // var divToDisplay = '<div id="leftDivHover">' + image+'</div>'
//        $this.addClass('sbk_hoverTooltipStyle');
//        $this.qtip(
//            {
//                overwrite: false,
//                content: {
//                    //Get the title from the nested img element's alt attribute
//                    title: $this.find('img').attr('alt'),
//                    text: '<div><div style=\"display:inline; float:left; margin-right: 5px;\"><img src="'+$(this).find('img').attr('src')+'"/></div><div style=\"display:inline;\">'+$this.attr('title')+'</div></div>'
    

//                },
      
//                style: {
                    
//                    title:{'color':'red'},
//                    name : 'Bootstrap',
//                //    classes: { tooltip: 'sbk_hoverTooltipStyle' },
//                    tip: {
//                        corner: 'left center',
//                        border: 1,
//                        width: 22,
//                        height: 11
//                    }, // Give it a speech bubble tip
                   
//                },
             
//                border:{color:'red'},
                
//                position: {
//                   my: "left center",
//                   at: "right center",
//                    viewport: $(window)
//                },
//                show: {
//                    delay:0,
//                    mouseover: true,
//                    solo:true
                            
//                    },
//                hide: {
//                    fixed:true
//                }
     
                    
                
//            });
//    });
//});

$(document).ready(function () {
    
    
      $('[id*=sbkAghsw_CollectionButtonImg]').each(function () {
    
          var $this = $(this);
          var cssClass = 'sbkAghsw_CollectionButtonTxt';
           var hovercardTitle = '<div style=\"display:inline; float:left; font-weight:bold;margin-left:70px;margin-top:-10px;\" class=\"'+cssClass+'\"><a href=' + $this.find('a').attr('href')+'>' + $this.find('img').attr('alt') + '</a></div><br/>';
        //   var hovercardHTML = '<div style=\"display:inline;margin:70px;\">' + $this.find('.spanHoverText').text() + '</div><br/><div ><a href='+$this.find('a').attr('href')+'>Go to the collection</a></div>';
        var hovercardHTML = '<div style=\"display:inline;margin:70px;\">' + $this.find('.spanHoverText').text() + '</div><br/>';
        

        $this.hovercard({
            detailsHTML: hovercardTitle+hovercardHTML,
            width: 300,
            openOnLeft: false,
            autoAdjust: false,
            delay:0
            
 
        });
       

    });
});