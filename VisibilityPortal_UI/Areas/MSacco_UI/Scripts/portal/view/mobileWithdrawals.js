var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Mobile Withdrawal Records Found ",
    pagination: "remote", //enable remote pagination
    paginationSize: 100,
    paginationSizeSelector: true,
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
      { title: "Entry No", field: "Entry_No", headerFilter: true },
      {
        title: "Status", field: "Status",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      { title: "Telephone No", field: "Telephone_No", headerFilter: true },
      {
        title: "Comments", field: "Comments",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      { title: "Account Name", field: "Account_Name", headerFilter: true },
      {
        title: "Trx Date", field: "Transaction_Date",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      { title: "Amount", field: "Amount", headerFilter: true },
      { title: "Account Balance", field: "Account_Balance", headerFilter: true },
      { title: "Request Confirmed", field: "RequestConfirmed", headerFilter: true },
      { title: "Account No", field: "Account_No", headerFilter: true },
      
      { title: "MPESA Result Type", field: "MPESA_Result_Type", headerFilter: true },
      { title: "MPESA Result Desc", field: "MPESA_Result_Desc", headerFilter: true },
      {
        title: "MPESA DateTime", field: "MPESA_DateTime",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      { title: "MPESA Float Amount", field: "MPESA_Float_Amount", headerFilter: true },
      { title: "Document No", field: "Document_No", headerFilter: true },
      { title: "MPesa Name", field: "MPesa_Name", headerFilter: true },
      {
        title: "Sent to Journal?", field: "Sent_To_Journal",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      //{ title: "Session ID", field: "SESSION_ID", headerFilter: true },
      //{ title: "Corporate No", field: "Corporate_No" },
    ],
    movableColumns: true,
    index: "Entry_No",
    initialSort: [
      { column: "Entry_No", dir: "desc" }
    ],
    headerSortTristate: true,
    
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

function LoadData(restUrl, corporateNo) {
  if (corporateNo == '' || corporateNo == undefined) return;
  var ajaxParams = { clientCorporateNo: corporateNo };

  tabulatorAjaxUrlForReload = restUrl;
  tabulatorAjaxParamsForReload = ajaxParams

  ClearFilters();
  tblTabulator.setData(restUrl, ajaxParams);
}

function ReloadData() {
  tblTabulator.setData(tabulatorAjaxUrlForReload, tabulatorAjaxParamsForReload);
}
