var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Utility Payment Records Found ",
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
      { title: "Comments", field: "Comments", headerFilter: true },
      { title: "Account Name", field: "Account_Name", headerFilter: true },
      { title: "Utility Payment Type", field: "Utility_Payment_Type", headerFilter: true },
      {
        title: "Trx Date", field: "Transaction_Date",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      { title: "Utility Payment Account", field: "Utility_Payment_Account", headerFilter: true },
      { title: "Amount", field: "Amount", headerFilter: true },
      { title: "Account Balance", field: "Account_Balance", headerFilter: true },
      { title: "Request Confirmed", field: "Request_Confirmed", headerFilter: true },
      { title: "Account No", field: "Account_No", headerFilter: true },
      { title: "Response Status", field: "Response_Status", headerFilter: true },
      { title: "Response Value", field: "Response_Value", headerFilter: true },
      { title: "Utility Result Type", field: "Utility_Result_Type", headerFilter: true },
      { title: "Utility Received", field: "Utility_Received", headerFilter: true },
      { title: "Document No", field: "Document_No", headerFilter: true },
      //{ title: "Session ID", field: "SESSION_ID", headerFilter: true },
      {
        title: "Sent to Journal?", field: "Sent_To_Journal",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
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
  