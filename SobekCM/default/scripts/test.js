function toServer() {
    var data = "action|test|";
    var dataPackage = data + "~";
    jQuery('form').each(function () {
        var payload = JSON.stringify({ sendData: dataPackage });
        var hiddenfield = document.getElementById('payload');
        hiddenfield.value = payload;
        var hiddenfield2 = document.getElementById('action');
        hiddenfield2.value = 'action';
        $.ajax({
            type: "POST",
            async: true,
            url: window.location.href.toString(),
            data: jQuery(this).serialize(),
            success: function (result) {
                //document.getElementById("testMe").innerHTML = result; //to json
                alert("done!");
            }
        });
    });
}