// Sacco dataset
var tblSaccoTabulator;
var saccoTabulatorAjaxUrlForReload;
var saccoTabulatorAjaxParamsForReload;

// CoreTec dataset
var tblCoretecTabulator;
var coretecTabulatorAjaxUrlForReload;
var coretecTabulatorAjaxParamsForReload;

function initSaccoTabulator(tableContainerID, editUrl) {
  //create Tabulator on DOM element with id == tableContainerID
  tblSaccoTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No User Records Found ",
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
      { title: "Client", field: "ClientName", headerFilter: true },
      { title: "First Name", field: "FirstName", headerFilter: true },
      { title: "Last Name", field: "LastName", headerFilter: true },
      { title: "Phone Number", field: "PhoneNumber", headerFilter: true },
      { title: "Email", field: "Email", headerFilter: true },
      {
        title: "Email Confirmed?", field: "EmailConfirmed",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Default Password?", field: "IsDefaultPassword",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Date Email Confirmed", field: "DateEmailConfirmed",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      {
        title: "Date Created", field: "CreatedOn",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      }      
      //{ title: "Corporate No", field: "ClientCorporateNo" },
    ],
    movableColumns: true,
    index: "ClientCorporateNo",
    groupBy: "ClientName",
    groupToggleElement: "header", //toggle group on click anywhere in the group header
    groupStartOpen: false,
    initialSort: [
      { column: "ClientName", dir: "asc" }
    ],
    headerSortTristate: true,

  });
  $(tblSaccoTabulator.element).addClass("table table-striped table-condensed table-hover");
}
function initCoretecTabulator(tableContainerID, editUrl) {
  //create Tabulator on DOM element with id == tableContainerID
  tblCoretecTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No User Records Found ",
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
      { title: "First Name", field: "FirstName", headerFilter: true },
      { title: "Last Name", field: "LastName", headerFilter: true },
      { title: "Email", field: "Email", headerFilter: true },
      {
        title: "Email Confirmed?", field: "EmailConfirmed",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Default Password?", field: "IsDefaultPassword",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Date Email Confirmed", field: "DateEmailConfirmed",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      {
        title: "Date Created", field: "CreatedOn",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      { title: "Created By", field: "CreatedBy", headerFilter: true }
    ],
    movableColumns: true,
    index: "Email",
    initialSort: [
      { column: "FirstName", dir: "asc" }
    ],
    headerSortTristate: true,

  });
  $(tblCoretecTabulator.element).addClass("table table-striped table-condensed table-hover");
}


function GetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function ClearFilters(dataSetName) {
  switch (dataSetName) {
    case "SaccoUsers":
      tblSaccoTabulator.clearFilter(true);
      break;
    case "CoretecUsers":
      tblCoretecTabulator.clearFilter(true);
      break;
  }
}

function LoadSaccoData(restUrl, corporateNo=null) {
  var ajaxParams = { clientCorporateNo: corporateNo };

  saccoTabulatorAjaxUrlForReload = restUrl;
  saccoTabulatorAjaxParamsForReload = ajaxParams

  ClearFilters("SaccoUsers");
  tblSaccoTabulator.setData(saccoTabulatorAjaxUrlForReload, saccoTabulatorAjaxParamsForReload);
}
function LoadCoretecData(restUrl) {
  var ajaxParams = { clientCorporateNo: "CORETEC" };

  coretecTabulatorAjaxUrlForReload = restUrl;
  coretecTabulatorAjaxParamsForReload = ajaxParams

  ClearFilters("CoretecUsers");
  tblCoretecTabulator.setData(coretecTabulatorAjaxUrlForReload, coretecTabulatorAjaxParamsForReload);
}

function ReloadData(dataSetName) {
  switch (dataSetName) {
    case "SaccoUsers":
      tblSaccoTabulator.setData(saccoTabulatorAjaxUrlForReload, saccoTabulatorAjaxParamsForReload);
      break;
    case "CoretecUsers":
      tblCoretecTabulator.setData(coretecTabulatorAjaxUrlForReload, coretecTabulatorAjaxParamsForReload);
      break;
  }
}

function initIonTabs(ionTabsRef) {
  $.ionTabs(ionTabsRef);
}
function initResponsiveTabs(tabsContainerRef) {
  $(tabsContainerRef).responsiveTabs({
    startCollapsed: 'accordion'
  });
}