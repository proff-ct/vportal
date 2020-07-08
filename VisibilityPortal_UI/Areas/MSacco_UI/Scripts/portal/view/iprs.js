var tblTabulatorIPRSLookup;
var tabulatorIPRSLookupAjaxUrlForReload;
var tabulatorIPRSLookupAjaxParamsForReload;

var txtSearchIdNumber;
var txtSearchPhoneNo;

function initTabulator(tableContainerID) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulatorIPRSLookup = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No IPRS Record Found ",
    pagination: false, // setting this to local v4.4 has buggy behaviour in which not all columns show
    //paginationSize: 5,
    //paginationSizeSelector: true,
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
    //layout: "fitDataFill", //fit columns to width of table (optional),
    layout: "fitColumns", 
    layoutColumnsOnNewData: true,
    //headerFilterPlaceholder: "filter ...",
    tooltipsHeader: true,
    columns: [ //Define Table Columns
      {
        formatter: "responsiveCollapse", width: 30, minWidth: 30,
        align: "center", resizable: false, headerSort: false
      },
      { title: "Status", field: "Status", headerFilter: false, headerSort: false },
      { title: "Surname", field: "Surname", headerFilter: false, headerSort: false },
      { title: "Other Names", field: "Othernames", headerFilter: false, headerSort: false },
      {
        title: "Date Of Birth", field: "Date_Of_Birth", headerFilter: false, headerSort: false,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      { title: "Citizenship", field: "Citizenship", headerFilter: false, headerSort: false },
      { title: "Gender", field: "Gender", headerFilter: false, headerSort: false },
    ],
    movableColumns: true

  });
  $(tblTabulatorIPRSLookup.element).addClass("table table-striped table-condensed table-hover");

}

function GetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function ClearFilters() {
  tblTabulatorIPRSLookup.clearFilter(true);
}

function LoadData(restUrl, corporateNo, getAll = false) {
  if (!getAll && (corporateNo == '' || corporateNo == undefined)) return;
  var idNum = $(txtSearchIdNumber).val();
  var phoneNo = $(txtSearchPhoneNo).val();
  if (idNum == '' || idNum == undefined) return;
  if (phoneNo == '' || phoneNo == undefined) {
    alert("Phone Number not specified");
    return;
  };
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    nationalIDNo: idNum,
    phoneNo: phoneNo,
    loadAll: getAll
  };

  tabulatorIPRSLookupAjaxUrlForReload = restUrl;
  tabulatorIPRSLookupAjaxParamsForReload = ajaxParams

  ClearFilters();
  tblTabulatorIPRSLookup.setData(restUrl, ajaxParams);
}

function ReloadData() {
  tblTabulatorIPRSLookup.setData(tabulatorIPRSLookupAjaxUrlForReload, tabulatorIPRSLookupAjaxParamsForReload);
}


function SortIPRSData(tabulatorTableRef) {
  tabulatorTableRef.setSort([
    { column: "Corporate_Name", dir: "asc" }, //sort by this first
    { column: "Http_Status", dir: "desc" }, //then sort by this second
  ]);
}

/**
 * MatAdo 02-Jan-2020: Code note:
 * Added some different styles for invoking functions e.g.
 * 1. SortIPRSData(tabulatorTableRef) 
 *      -> called and passed 'this'
 * 2. ReloadData = ReloadDataTabulator.bind(tblTabulatorIPRSLookup);
 *      -> is bound to the tblTabulatorIPRSLookup object so I don't have to use the variable name
 * These are in readiness for sprucing up the js code now that we're nearing full on deployment to
 * production. 
 * The point of them therefore is to help refresh the memory when next I revisit this code
 * 
 **/ 