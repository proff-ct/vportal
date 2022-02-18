(function () {
    var msaccoClient = $.connection.msaccoClient;
    $.connection.hub.logging = true;
    $.connection.hub.start()
        .done(function () { console.log("Connected to MSACCO | " + moment().format(PORTAL_DATE_FORMAT) + " | Connection ID: " + $.connection.hub.id); })
        .fail(function () { console.log("Failed connecting to MSACCO | " + moment().format(PORTAL_DATE_FORMAT)) });

    msaccoClient.client.serverMessage = function (apiCommParams, response) {
        if (response == '') {
            response = { success: false, ex: "Server did not provide a response" };
        }
        else {
            try {
                response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
            }
            catch (err) {
                console.log(
                    "Error processing disconnected callback. Failed interpreting response: " + err.message + " at " + moment().format(PORTAL_DATE_FORMAT));
                response = { success: false, ex: "An error occurred interpreting the server's response. Kindly try again" };
            }
        }
        // Display message as toast
        bootbox.alert({
            title: "<h4>MSACCO</h4>",
            message: response.ex
        });
    }

})();

var requestType = {
    GET: 'GET',
    POST: 'POST'
};

var ERR_CODE = {
    BAD_REQ: 400
};

var msaccoCallBack = {
    SUCCESS: function (response) { },
    ERROR: function (xhr, status, error) { }
};



function MSACCODecryptor(secretKey, token, dataToDecrypt) {
    var key = CryptoJS.enc.Utf8.parse(secretKey);
    var iv = CryptoJS.enc.Utf8.parse(token);

    var decrypted = CryptoJS.AES.decrypt(dataToDecrypt, key, { iv: iv });

    return decrypted.toString(CryptoJS.enc.Utf8);
}

function CallMSACCO(restUrl, reqData, apiCommParams, callbackSpec) {
    $.ajax(restUrl, {
        type: apiCommParams.requestType,
        contentType: "application/x-www-form-urlencoded",
        data: reqData,
        dataType: "json",
        headers: {
            'XToken': apiCommParams.xToken
        },
        success: callbackSpec.SUCCESS,
        error: callbackSpec.ERROR
    });
}
