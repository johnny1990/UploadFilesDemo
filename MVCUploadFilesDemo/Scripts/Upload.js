$('#Upload').click(function () {

    var fileUpload = $("#Files").get(0);
    var files = fileUpload.files;
   
    var fileData = new FormData();

    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }
    $.ajax({
        url: '/Home/UploadFiles',
        type: "POST",
        contentType: false, 
        processData: false, 
        data: fileData,
        async: false,
        success: function (result) {
            if (result != "") {
                $('#BrowseFile').find("*").prop("disabled", true);

            }
        },
        error: function (err) {
            alert(err.statusText);
        }
    });

}); 