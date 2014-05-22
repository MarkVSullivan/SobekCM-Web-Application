
//Code using JQuery qTip
$(document).ready(function() {
  //  alert('calling this function successfully');
    $('[id*=sbkThumbnailImg').each(function () {
        var $this = $(this);
        var title_count = $this.attr('id').split('g')[1];
  //      alert(title_count);
        //    var divContent = document.getElementById('descThumbnail' + title_count);
        var divContent = $('#descThumbnail' + title_count).html();
    //    var divContentText = divContent.html();
     //   alert(divContent);
        var titleText = '<div style="color:red;"><b>' + $this.parent().parent().parent().parent().next().find('span').html() + '</b></div>';

  //      alert(titleText);
         $this.qtip(
            {
             //   title: titleText,
                content: {
             //       title: titleText,
                    text:  divContent
                },
                position: {
                    my: 'center left',
                    at: 'center right'
                },
                style: {
                    width: 500,
                    //        height: 200,
                    border: 1,
                    
                    hide: {
                        fixed: true,
                        delay:1000
                    },
                    classes: 'qtip-tipsy',
                    tip: {
                        width: 20
                    }
                }
    
            });   
              
   // alert($this.prop('tagName'));

            });
    });




//$(document).ready(function () {

//    $('[id*=sbkThumbnailSpan').each(function() {
//        var $this = $(this);
//    //    alert($this);
//        $this.tooltip({
//            tooltipClass: "tooltipThumbnailResults",
//            //    $(document).tooltip({
//             items: "img",
//            position: { my: 'left center', at: 'right+110px center'},

//            content: function() {
//                var element = $(this);

//    //            alert(element.parent().parent().parent().parent().next().find('span').html());
//       //         alert(element.prop('tagName')); - IMG
//                var title = element.parent().parent().parent().parent().next().find('span').html();
//   //             alert('Next span text:'+thisContent);
//                //  return ('Test content');
//                return (title);
//            }
//        });
//    });
//});
