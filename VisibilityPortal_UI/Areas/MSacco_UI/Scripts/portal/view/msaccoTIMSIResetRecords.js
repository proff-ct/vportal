var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID) {
  var dateFieldAccessor = function (value, data, type, params, column) {
    return GetFormattedDate(value);
  };
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Logged Authentication Record(s) Found ",
    pagination: "remote", //enable remote pagination
    paginationSize: 100,
    paginationSizeSelector: [100, 300, 700, 1000],
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
    layout: "fitColumns", //"fitDataFill", //fit columns to width of table (optional),
    headerFilterPlaceholder: "filter ...",
    tooltipsHeader: true,
    columns: [ //Define Table Columns
      {
        formatter: "responsiveCollapse", width: 30, minWidth: 30,
        align: "center", resizable: false, headerSort: false
      },
      //{ title: "Subscriber ID", field: "LogNo" },
      { title: "Member Phone Number", field: "CustomerPhoneNo", headerFilter: true },
      { title: "Authenticated By", field: "ActionUser", headerFilter: true },
      { title: "Supplied KYC Info", field: "ResetNarration", formatter: "textarea", headerFilter: true},
      {
        title: "TIMESTAMP", field: "ResetStatusDate",
        align: "center",
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        },
        accessorDownload: dateFieldAccessor,
        accessorDownloadParams: {}
      },

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

function ExportExcel() {
  var excelFileName = "IMSI_Authentication_Logs_" + moment().format("DD-MMM-YYYY_HHmmss") + ".xlsx";
  tblTabulator.download("xlsx", excelFileName, {
    sheetName: "Authenticated IMSI Records" 
  });
}