var tblTabulatorLinkMonitoring;
var tabulatorLinkMonitoringAjaxUrlForReload;
var tabulatorLinkMonitoringAjaxParamsForReload;

function initTabulatorLinkMonitoring(tableContainerID) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulatorLinkMonitoring = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Link Status Records Found ",
    pagination: "remote", // setting this to local v4.4 has buggy behaviour in which not all columns show
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
      { title: "Sacco", field: "Corporate_Name", headerFilter: true },
      { title: "Http Link Status", field: "Http_Status", headerFilter: true },
      {
        title: "Time Last Checked", field: "Last_Check",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return LinkMonitoringGetFormattedDate(cell.getValue());
        }
      },
      //{
      //  title: "Time Last Email Sent", field: "Last_Email_Sent",
      //  align: "center", headerFilter: true,
      //  formatter: function (cell, formatterParams) {
      //    return LinkMonitoringGetFormattedDate(cell.getValue());
      //  }
      //},
    ],
    movableColumns: true,
    index: "Corporate_Name",
    initialSort: [
      { column: "Http_Status", dir: "desc" },
      { column: "Corporate_Name", dir: "asc" },
    ],
    headerSortTristate: true,
  });
  $(tblTabulatorLinkMonitoring.element).addClass("table table-striped table-condensed table-hover");
}

function LinkMonitoringGetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function LinkMonitoringClearFilters() {
  tblTabulatorLinkMonitoring.clearFilter(true);
}

function LoadTabulatorLinkMonitoringData(restUrl, corporateNo, getAll = false) {
  if (!getAll && (corporateNo == '' || corporateNo == undefined)) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    loadAll: getAll
  };

  tabulatorLinkMonitoringAjaxUrlForReload = restUrl;
  tabulatorLinkMonitoringAjaxParamsForReload = ajaxParams

  LinkMonitoringClearFilters();
  tblTabulatorLinkMonitoring.setData(restUrl, ajaxParams);
}

function LinkMonitoringReloadData() {
  tblTabulatorLinkMonitoring.setData(tabulatorLinkMonitoringAjaxUrlForReload, tabulatorLinkMonitoringAjaxParamsForReload);
}
