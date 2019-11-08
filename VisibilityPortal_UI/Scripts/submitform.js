

$("#form").submit(function (e) {

    e.preventDefault(); // avoid to execute the actual submit of the form.

    var form = $(this);
    var url = form.attr('action');
    showLoader();
    $.ajax({
        type: "POST",
        url: url,
        data: form.serialize(), // serializes the form's elements.
        success: function (data) {
            showModal(data);
            setTimeout(function() {
                location.reload();
            },2000);
           
            console.log(data);
        },
        error: function (data) {
            showModal(data);
            console.log(data);
        }
    });

});
function showModal(data) {

    var message = JSON.parse(data);
    $('#modal-title').text(message.Code);
    $('#modal-message').text(message.Message);
    if (message.Code === "500" || message.Code.includes("error")) {
        $('#modal-bg').addClass("bg-danger");
    } else {
        $('#modal-bg').addClass("bg-success");
    }
    $("#alert").modal("show");
    stopLoader();

}
function showLoader() {
    document.getElementById("overlay").style.display = "block";
}

function stopLoader() {
    document.getElementById("overlay").style.display = "none";
}