var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, apiCommParams) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Device Record(s) Found ",
    //pagination: "remote", //enable remote pagination
    //paginationSize: 100,
    //paginationSizeSelector: true,
    //ajaxProgressiveLoad: "scroll",
    //ajaxURL: 'Loans/GetLoanRecords',
    ajaxResponse: function (url, params, response) {
      //url - the URL of the request
      //params - the parameters passed with the request
      //response - the JSON object returned in the body of the response.

      response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
      return response;
    },
    // collapse columns that no longer fit on the table into a list under the row
    responsiveLayout: "collapse",
    responsiveLayoutCollapseStartOpen: false,
    layout: "fitDataFill", //fit columns to width of table (optional),
    headerFilterPlaceholder: "filter ...",
    tooltipsHeader: true,
    columns: [ //Define Table Columns
      {
        formatter: "responsiveCollapse", width: 30, minWidth: 30,
        align: "center", resizable: false, headerSort: false
      },
      { title: "Member Phone Number", field: "PhoneNumber" },
      { title: "DeActivation Reason", field: "Comments" },
      {
        title: "Last Activation Date", field: "DateLinked",
        align: "center",
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      {
        title: "Last Updated On", field: "LastUpdated",
        align: "center",
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      }      
      
      //{ title: "Session ID", field: "SESSION_ID", headerFilter: true },
      //{ title: "Corporate No", field: "Corporate_No" },
    ],
    movableColumns: true
    
  });
  $(tblTabulator.element).addClass("table table-striped table-condensed table-hover");
}


function GetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function ClearFilters() {
  tblTabulator.clearFilter(true);
}

function SearchMemberDeviceRecord(restUrl, corporateNo, phoneNo) {
  if (corporateNo == '' || corporateNo == undefined || phoneNo == '' || phoneNo == undefined) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    memberTelephoneNo: phoneNo
  };

  tabulatorAjaxUrlForReload = restUrl;
  tabulatorAjaxParamsForReload = ajaxParams

  ClearFilters();
  tblTabulator.setData(restUrl, ajaxParams);
}

function ReloadData() {
  tblTabulator.setData(tabulatorAjaxUrlForReload, tabulatorAjaxParamsForReload);
}

function ResetMemberDevice(restUrl, corporateNo, phoneNo, apiCommParams) {
  if (corporateNo == '' || corporateNo == undefined || phoneNo == '' || phoneNo == undefined) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    memberTelephoneNo: phoneNo
  };
  apiCommParams.requestType = requestType.POST;

  msaccoCallBack.SUCCESS = function (response) {
    response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
    ParseActivationResponse(response);
  };

  msaccoCallBack.ERROR = function (xhr, status, error) {
    var msg = { success: false, ex: null };
    var errorCode = xhr.status;

    if (errorCode == ERR_CODE.BAD_REQ) {
      if (error) {
        msg.ex = "MSACCO says: " + error + "<p/><p/>Kindly log out then log back in to resolve this error";
      }
      else msg.ex = "MSACCO returned an error<p/><p/>Kindly log out then log back in to resolve.";
    }
    else if (errorCode == 200 && status == "parsererror") {
      msg.ex = "Server error. <p/><p/>Close all portal tabs then log out and log back in.";
    }
    else msg.ex = "An error occurred communicating with MSACCO. Kindly try again";

    ParseActivationResponse(msg);
  };

  CallMSACCO(restUrl, ajaxParams, apiCommParams, msaccoCallBack);
}

function ParseActivationResponse(serverResponse) {
  var msg;
  if (serverResponse.success == true) {
    msg = "<h4>Status: Success</h4>  <p/><p/>" + serverResponse.ex + "<p/><p/>Member can now login via MSACCO+ Mobile App"
  } else {
    msg = "<h4>Status: Failed</h4> <p/><p/>" + serverResponse.ex;
  }

  bootbox.alert({
    title: "<h4>MSACCO+ Device (Re)Activation</h4>",
    message: msg
  });
}
