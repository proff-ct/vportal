function PrevalidateLogin(portalLoginUrl, emailAddress) {
    $.post(portalLoginUrl, { email: emailAddress }, function (data) {
        if (data.success == true) {
            return true;
        }
        else return false;
    })
    .fail(function () {
        return false;
    });
}