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