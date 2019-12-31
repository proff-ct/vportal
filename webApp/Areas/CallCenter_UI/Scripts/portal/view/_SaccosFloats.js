var tblTabulatorSaccoFloats;
var tabulatorSaccoFloatsAjaxUrlForReload;
var tabulatorSaccoFloatsAjaxParamsForReload;

function initTabulatorFloat(tableContainerID) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulatorSaccoFloats = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Float Records Found ",
    pagination: "local", // setting this to local v4.4 has buggy behaviour in which not all columns show
    paginationSize: 100,
    paginationSizeSelector: true,
    // DO NOT USE any of the peristence settings! Matthew Adote 08NOV2019_0842
    //persistenceMode: true, // this is not to be used
    //persistentLayout: true,
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
      //{ title: "Corporate No", field: "Corporate_No", visible: "false" },
      { title: "Sacco", field: "SaccoName", headerFilter: true },
      {
        title: "M-Pesa Float", field: "MPesaFloat", headerFilter: true,
        formatter: function (cell) {
          return SetCellValueToNullIfNoData(cell.getValue())
        }
      },
      {
        title: "BULK SMS Float", field: "BulkSMSFloat", headerFilter: true,
        formatter: function (cell) {
          return SetCellValueToNullIfNoData(cell.getValue())
        }
      },
      {
        title: "M-Pesa Date", field: "MpesaFloatDate",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return SaccoFloatsGetFormattedDate(cell.getValue());
        }
      },
      {
        title: "BULK SMS Date", field: "BulkSMSFloatDate",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return SaccoFloatsGetFormattedDate(cell.getValue());
        }
      }
    ],
    movableColumns: true,
    index: "SaccoName",
    initialSort: [
      { column: "SaccoName", dir: "asc" }
    ],
    headerSortTristate: true,
  });
  $(tblTabulatorSaccoFloats.element).addClass("table table-striped table-condensed table-hover");
}

function SaccoFloatsGetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function SetCellValueToNullIfNoData(cellValue, valueToSet = null) {
  return (cellValue == null) ? (valueToSet != null) ? valueToSet : "No Data" : cellValue
}

function SaccoFloatsClearFilters() {
  tblTabulatorSaccoFloats.clearFilter(true);
}

function LoadSaccoFloatsData(restUrl, corporateNo, getAll = false) {
  if (!getAll && (corporateNo == '' || corporateNo == undefined)) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    loadAll: getAll
  };

  tabulatorSaccoFloatsAjaxUrlForReload = restUrl;
  tabulatorSaccoFloatsAjaxParamsForReload = ajaxParams

  SaccoFloatsClearFilters();
  tblTabulatorSaccoFloats.setData(restUrl, ajaxParams);
}

function SaccoFloatsReloadData() {
  tblTabulatorSaccoFloats.setData(tabulatorSaccoFloatsAjaxUrlForReload, tabulatorSaccoFloatsAjaxParamsForReload);
}
