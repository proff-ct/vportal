const SACCO_VITALS = {
  Floats: {
    MPESA: "MPESA",
    BULK_SMS: "BULK_SMS",
    TRX_SMS: "TRX_SMS"
  },
  LinkStatus: "LinkStatus"
};

function RefreshSaccoVitals(saccoVitals, vitalToLoad = null) {
  if (vitalToLoad) {
    switch (vitalToLoad) {
      case SACCO_VITALS.Floats.MPESA:
        LoadData(saccoVitals.mpesaFloat.restUrl, saccoVitals.corporateNo,
          saccoVitals.mpesaFloat.contentContainer, SACCO_VITALS.Floats.MPESA);
        break;
      case SACCO_VITALS.Floats.BULK_SMS:
        LoadData(saccoVitals.bulkSMSFloat.restUrl, saccoVitals.corporateNo,
          saccoVitals.bulkSMSFloat.contentContainer, SACCO_VITALS.Floats.BULK_SMS);
        break;
      case SACCO_VITALS.Floats.TRX_SMS:
        LoadData(saccoVitals.trxSMSFloat.restUrl, saccoVitals.corporateNo,
          saccoVitals.trxSMSFloat.contentContainer, SACCO_VITALS.Floats.TRX_SMS);
        break;
      case SACCO_VITALS.LinkStatus:
        LoadData(saccoVitals.linkStatus.restUrl, saccoVitals.corporateNo,
          saccoVitals.linkStatus.contentContainer, SACCO_VITALS.LinkStatus);
        break;
    }
  } else {
    // saccoVitals object not passed in from front end so load everything
    for (key in SACCO_VITALS) {
      switch (key) {
        case "Floats":
          for (float in SACCO_VITALS.Floats) {
            switch (float) {
              case SACCO_VITALS.Floats.MPESA:
                LoadData(saccoVitals.mpesaFloat.restUrl, saccoVitals.corporateNo,
                  saccoVitals.mpesaFloat.contentContainer, SACCO_VITALS.Floats.MPESA);
                break;
              case SACCO_VITALS.Floats.BULK_SMS:
                LoadData(saccoVitals.bulkSMSFloat.restUrl, saccoVitals.corporateNo,
                  saccoVitals.bulkSMSFloat.contentContainer, SACCO_VITALS.Floats.BULK_SMS);
                break;
              case SACCO_VITALS.Floats.TRX_SMS:
                LoadData(saccoVitals.trxSMSFloat.restUrl, saccoVitals.corporateNo,
                  saccoVitals.trxSMSFloat.contentContainer, SACCO_VITALS.Floats.TRX_SMS);
                break;
            };
          };
          break;
        case "LinkStatus":
          LoadData(saccoVitals.linkStatus.restUrl, saccoVitals.corporateNo,
            saccoVitals.linkStatus.contentContainer, SACCO_VITALS.LinkStatus);
          break;
      }
    };
  }
}

function LoadData(restUrl, corporateNo, containerRef, vitalToGet) {
  $.ajax({
    method: "GET",
    //dataType: 'json',
    url: restUrl,
    data: {
      clientCorporateNo: corporateNo,
      vitalToLoad: vitalToGet
    },
    success: function (responseData) {
      // determine what to do depending on the vital
      switch (vitalToGet) {
        case SACCO_VITALS.LinkStatus:
          var pingResult = responseData.data.Ping_Result;
          var httpStatus = responseData.data.Http_Status;

          var linkStatus = pingResult=="True" ? "UP" : "Unknown";
          $(containerRef).text(linkStatus);
          break;
        default:
          $(containerRef).text(responseData.data)
      };
    },
    error: function (xhr, ajaxOptions, thrownError) {
      alert("Error occurred");
      $(containerRef).text(thrownError);
    }
  });
}
