var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, apiCommParams) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No USSD Logs Found ",
    pagination: "remote", //enable remote pagination
    paginationSize: 200,
    paginationSizeSelector: true,
    //ajaxProgressiveLoad: "scroll",
    //ajaxURL: 'Loans/GetLoanRecords',
    ajaxResponse: function (url, params, response) {
      //url - the URL of the request
      //params - the parameters passed with the request
      //response - the JSON object returned in the body of the response.

      //return response.data; //return the tableData property of a response json object
      response.data = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response.data));
      return response;
    },
    // collapse columns that no longer fit on the table into a list under the row
    responsiveLayout: "collapse",
    responsiveLayoutCollapseStartOpen: false,
    layout: "fitColumns",//"fitDataFill", //fit columns to width of table (optional),
    headerFilterPlaceholder: "filter ...",
    tooltipsHeader: true,
    groupToggleElement: "header",
    groupBy: ["Telephone_No", "Session_ID"],
    groupStartOpen: [false, true],
    columns: [ //Define Table Columns
      {
        formatter: "responsiveCollapse", width: 30, minWidth: 30,
        align: "center", resizable: false, headerSort: false
      },
      //{ title: "Entry No", field: "Entry_No", headerFilter: true },
      //{
      //  title: "Session ID", field: "Session_ID",
      //  headerFilter: "select", headerFilterFunc: "=",
      //  headerFilterParams: { values: true }
      //},
      { title: "S.No", field: "OrderNo", width: 50, minWidth: 50, headerSort: false },
      { title: "Telephone No", field: "Telephone_No", width: 140, frozen: true, headerFilter: true },
      { title: "MSACCO Prompt", field: "MSACCOResponse", formatter: "textarea", headerFilter: true },
      { title: "User Response", field: "User_Input", width: 140, align: "center", headerFilter: true },
      {
        title: "Response Timestamp", field: "DateTime",
        align: "center",
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
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
  $('#memberTelNo').val('');
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

function SearchMemberUSSDLogs(restUrl, corporateNo, phoneNo) {
  if (corporateNo == '' || corporateNo == undefined || phoneNo == '' || phoneNo == undefined) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    memberTelephoneNo: phoneNo
  };

  //tabulatorAjaxUrlForReload = restUrl;
  //tabulatorAjaxParamsForReload = ajaxParams

  ClearFilters();
  tblTabulator.clearData();
  tblTabulator.setData(restUrl, ajaxParams);
}