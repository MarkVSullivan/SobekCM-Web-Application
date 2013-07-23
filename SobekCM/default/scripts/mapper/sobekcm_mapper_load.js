//$(document).ready(function () {
//    // Add the page method call as an onclick handler for the div.
//    var scriptURL = "default/scripts/serverside/Scripts.aspx";
//    de("Debug ID: 1");
//    $("#Result").click(function () {
//        $.ajax({
//            type: "POST",
//            url: baseURL+scriptURL+"/GetDate",
//            data: "{}",
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            success: function (msg) {
//                // Replace the div's content with the page method's return.
//                $("#Result").text(msg.d);
//            }
//        });
//    });
//});