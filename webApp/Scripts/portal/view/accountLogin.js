function PrevalidateLogin(portalLoginUrl, emailAddress, callback) {
    $.post(portalLoginUrl, { email: emailAddress }, function (data) {
        if (data.success == true) {
            callback(true);
        }
        else callback(false);
    })
    .fail(function () {
        callback(false);
    });
}