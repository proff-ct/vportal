var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID) {
  // init custom formatters
  Tabulator.prototype.extendModule("format", "formatters", {
    trxAmount: function (cell, formatterParams) {
      var moneyFormatter = Tabulator.prototype.moduleBindings.format.prototype.formatters.money;
      var floatCellValue = TabulatorSetCellValueToNullIfNoData(cell.getValue());

      var parsedFloatValue = parseFloat(floatCellValue);
      if (isNaN(parsedFloatValue)) {
        return floatCellValue;
      }

      //update the cell value before passing it on to the inbuilt money formatter
      //cell._cell.value = parsedFloatValue;
      cell.value = parsedFloatValue;
      return moneyFormatter(cell, formatterParams)
    }
  });
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Bank Transfer Records Found ",
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
      { title: "Telephone #", field: "Telephone_No", headerFilter: true },
      { title: "Sacco Account #", field: "Account_No", headerFilter: true },
      { title: "Bank Account #", field: "Recipient_Account_No", headerFilter: true },
      {
        title: "Bank Name", field: "Bank_Name",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      { title: "Narration", field: "Narration", headerFilter: true },
      {
        title: "Trx Date", field: "TransactionDate",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      {
        title: "Amount", field: "Amount", headerFilter: true,
        formatter: "trxAmount",
        formatterParams: {
          symbol: "KES "
        }
      },
      {
        title: "Sent to Journal?", field: "Sent_To_Journal",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      //{ title: "Account Balance", field: "AccountBalance", headerFilter: true },
      { title: "Request Confirmed", field: "Request_Confirmed", headerFilter: true },
      { title: "Comments", field: "Comments", headerFilter: true },
      //{ title: "Session ID", field: "SESSION_ID", headerFilter: true },
      //{ title: "Corporate No", field: "Corporate_No" },
    ],
    movableColumns: true,
    index: "Entry_No",
    initialSort: [
      { column: "TransactionDate", dir: "desc" }
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
  