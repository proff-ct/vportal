var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID) {
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
    //ajaxResponse: function (url, params, response) {
    //  //url - the URL of the request
    //  //params - the parameters passed with the request
    //  //response - the JSON object returned in the body of the response.

    //  return response.data; //return the tableData property of a response json object
    //},
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
      { title: "TIMSI ID", field: "TimsiId" },
      { title: "Member Phone Number", field: "PhoneNumber" },
      { title: "Reason Blocked", field: "Comments" },
      {
        title: "Initial Date Linked", field: "DateLinked",
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

function SearchMemberTIMSIRecord(restUrl, corporateNo, phoneNo) {
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

function ResetMemberDevice(restUrl, corporateNo, phoneNo, trustReason) {
  if (corporateNo == '' || corporateNo == undefined || phoneNo == '' || phoneNo == undefined || trustReason == '' || trustReason == undefined) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    memberTelephoneNo: phoneNo,
    trustReason: trustReason
  };

  $.post(restUrl, ajaxParams, function (response) {
    var serverResponse;
    if (response.success == true) {
      serverResponse = "<h4>Status: Success</h4>  <p/><p/>Member can now access MSACCO"
    } else {
      serverResponse = "<h4>Status: Failed</h4> <p/><p/>" + response.ex;
    }

    bootbox.alert({
      title: "<h3>MSACCO TIMSI (Re)Activation</h3>",
      message: serverResponse
    })
  });
}