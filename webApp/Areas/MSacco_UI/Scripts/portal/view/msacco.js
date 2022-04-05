(function () {
    var msaccoClient = $.connection.msaccoClient;
    msaccoClient.client.serverMessage = function (apiCommParams, response) {
        if (response == '') {
            response = { success: false, ex: "Server did not provide a response", title: "<h5>MSACCO</h5>" };
        }
        else {
            try {
                apiCommParams = JSON.parse(apiCommParams);
                response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
            }
            catch (err) {
                console.log(
                    "Error processing disconnected callback. Failed interpreting response: " + err.message + " at " + moment().format(PORTAL_DATE_FORMAT));
                response = {
                    success: false,
                    ex: "An error occurred interpreting the server's response. <br/><br/>",
                    title: "<h5>MSACCO</h5>"
                };
            }
        }
        // Display message as toast
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": true,
            "progressBar": true,
            "positionClass": "toast-top-center",
            "preventDuplicates": true,
            "onclick": null,
            "showDuration": "10000",
            "hideDuration": "1000",
            "timeOut": "0",
            "extendedTimeOut": "0",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        if (response.success) {
            toastr["success"](response.ex, response.title);
        }
        else {
            toastr["error"](response.ex, response.title);
        }

    }

    $.connection.hub.logging = true;
    $.connection.hub.start()
        .done(function () { console.log("Connected to MSACCO | " + moment().format(PORTAL_DATE_FORMAT) + " | Connection ID: " + $.connection.hub.id); })
        .fail(function (error) { console.log("Failed connecting to MSACCO->"+error +" | " + moment().format(PORTAL_DATE_FORMAT)) });

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
