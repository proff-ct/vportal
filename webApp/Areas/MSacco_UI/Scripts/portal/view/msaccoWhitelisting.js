var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, apiCommParams) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Member Record(s) Found ",
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
      //{ title: "Reason Blocked", field: "Comments" },
      {
        title: "MSACCO Registration Date", field: "DateRegistered",
        align: "center",
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      }

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

function SearchMemberRecord(restUrl, corporateNo, phoneNo) {
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

function WhitelistMember(restUrl, corporateNo, phoneNo, trustReason, apiCommParams) {
  if (corporateNo == '' || corporateNo == undefined || phoneNo == '' || phoneNo == undefined || trustReason == '' || trustReason == undefined || apiCommParams == undefined) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    memberTelephoneNo: phoneNo,
    trustReason: trustReason
  };

  $.post(restUrl, ajaxParams, function (response) {
    var serverResponse;
    response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
    if (response.success == true) {
      serverResponse = "<h4>Status: Success</h4>  <p/><p/>" + response.ex +"<p/><p/>Member can now transact on MSACCO"
    } else {
      serverResponse = "<h4>Status: Failed</h4> <p/><p/>" + response.ex;
    }

    bootbox.alert({
      title: "<h3>MSACCO Whitelisting</h3>",
      message: serverResponse
    })
  });
}