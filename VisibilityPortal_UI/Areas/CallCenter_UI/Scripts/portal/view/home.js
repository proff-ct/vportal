const SACCO_VITALS = {
  Floats: {
    MPESA: "MPESA",
    BULK_SMS: "BULK_SMS",
    TRX_SMS: "TRX_SMS"
  },
  LinkStatus: "LinkStatus"
};

const CALLBACK_RESPONSE = {
  STATUS: {
    OK: "OK",
    ERROR: "ERR"
  }
};
function RefreshSaccoVitals(saccoVitals, callBack, vitalToLoad = null) {
  if (saccoVitals.corporateNo == "" || saccoVitals.corporateNo == null) return;
  var vitalRestUrl = null;
  var vitalLabel = null;

  if (vitalToLoad) {
    switch (vitalToLoad) {
      case SACCO_VITALS.Floats.MPESA:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.mpesaFloat.restUrl;
        break;
      case SACCO_VITALS.Floats.BULK_SMS:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.bulkSMSFloat.restUrl;
        break;
      case SACCO_VITALS.Floats.TRX_SMS:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.trxSMSFloat.restUrl;
        break;
      case SACCO_VITALS.LinkStatus:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.linkStatus.restUrl;
        break;
    }
    LoadData(vitalRestUrl, saccoVitals.corporateNo, vitalLabel, callBack);
  } else {
    // no specific vital passed in from front end so load everything
    for (key in SACCO_VITALS) {
      switch (key) {
        case "Floats":
          for (float in SACCO_VITALS.Floats) {
            switch (float) {
              case SACCO_VITALS.Floats.MPESA:
                vitalLabel = float;
                vitalRestUrl = saccoVitals.mpesaFloat.restUrl;
                break;
              case SACCO_VITALS.Floats.BULK_SMS:
                vitalLabel = float;
                vitalRestUrl = saccoVitals.bulkSMSFloat.restUrl;
                break;
              case SACCO_VITALS.Floats.TRX_SMS:
                vitalLabel = float;
                vitalRestUrl = saccoVitals.trxSMSFloat.restUrl;
                break;
            };
          };
          break;
        case "LinkStatus":
          vitalLabel = SACCO_VITALS.LinkStatus;
          vitalRestUrl = saccoVitals.linkStatus.restUrl;
          break;
      }
      LoadData(vitalRestUrl, saccoVitals.corporateNo, vitalLabel, callBack);
    };
  }
}

function LoadData(restUrl, corporateNo, vitalToGet, callBack) {
  $.ajax({
    method: "GET",
    //dataType: 'json',
    url: restUrl,
    data: {
      clientCorporateNo: corporateNo,
      vitalToLoad: vitalToGet
    },
    success: function (responseData) {
      var response = {};
      if (responseData === "" || responseData === null) {
        response = {
          status: CALLBACK_RESPONSE.STATUS.ERROR,
          error: "No data"
        };
      }
      else {
        response = {
          status: CALLBACK_RESPONSE.STATUS.OK,
          data: responseData.data
        };
      }
      callBack(vitalToGet, response);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      var response = {
        status: CALLBACK_RESPONSE.STATUS.ERROR,
        error: thrownError
      };
      callBack(vitalToGet, response);
    }
  });
}
