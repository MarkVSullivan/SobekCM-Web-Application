$(document).ready(function() {
    
    //$('#sbkAghsw_CollectionButtonImg0').qtip(
    $('[id*=sbkAghsw_CollectionButtonImg').each(function() {
        var $this = $(this);


     // var divToDisplay = '<div id="leftDivHover">' + image+'</div>'
        $this.addClass('sbk_hoverTooltipStyle');
        $this.qtip(
            {
                overwrite: false,
                content: {
                    //Get the title from the nested img element's alt attribute
                  title: $this.find('img').attr('alt'),
                  text: $this.attr('title')

                },
      
                style: {
                    classes: 'ui-tooltip-dark ui-tooltip-shadow',
                //    classes: { tooltip: 'sbk_hoverTooltipStyle' },
                    tip: {
                        corner: 'left center',
                        border: 1,
                        width: 22,
                        height: 11
                    }, // Give it a speech bubble tip
                   
                },
             
             
                
                position: {
                   my: "left center",
                   at: "right center",
                    viewport: $(window)
                },
                show: {
                    delay:0,
                    mouseover: true,
                    solo:true
                            
                    },
                hide: {
                    fixed:true
                }
     
                    
                
            });
    });
});

//$(document).ready(function () {
//    var testHTML = '<p>This is a test description</p>';
//    alert(testHTML);
//    $('#sbkAghsw_CollectionButtonImg0').hovercard({
//        detailsHTML: testHTML,
//        width: 400
//    });
//});
