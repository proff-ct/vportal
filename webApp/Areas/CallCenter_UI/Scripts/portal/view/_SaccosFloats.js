﻿var tblTabulatorSaccoFloats;
var tabulatorSaccoFloatsAjaxUrlForReload;
var tabulatorSaccoFloatsAjaxParamsForReload;

function initTabulatorFloat(tableContainerID) {
  // define my custom formatters here first
  Tabulator.prototype.extendModule("format", "formatters", {
    mpesa: function (cell, formatterParams) {
      var moneyFormatter = Tabulator.prototype.moduleBindings.format.prototype.formatters.money;
      var mpesaFloat = SetCellValueToNullIfNoData(cell.getValue());

      var parsedFloatValue = parseFloat(mpesaFloat);
      if (isNaN(parsedFloatValue)) {
        return mpesaFloat;
      }

      //update the cell value before passing it on to the inbuilt money formatter
      cell._cell.value = parsedFloatValue;
      return moneyFormatter(cell, formatterParams)
    },
    bulkSMS: function (cell, formatterParams) {
      var moneyFormatter = Tabulator.prototype.moduleBindings.format.prototype.formatters.money;
      var bulkSMSFloat = SetCellValueToNullIfNoData(cell.getValue());

      var parsedFloatValue = parseFloat(bulkSMSFloat);
      if (isNaN(parsedFloatValue)) {
        return bulkSMSFloat;
      }

      //update the cell value before passing it on to the inbuilt money formatter
      cell._cell.value = parsedFloatValue;
      return moneyFormatter(cell, formatterParams)
    },
  });

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
        //formatter: function (cell, formatterParams) {
        //  return SetCellValueToNullIfNoData(cell.getValue())
        //},
        formatter: "mpesa",
        formatterParams: {
          symbol: "KES"
        }
      },
      {
        title: "BULK SMS Float", field: "BulkSMSFloat", headerFilter: true,
        //formatter: function (cell) {
        //  return SetCellValueToNullIfNoData(cell.getValue())
        //}
        formatter: "bulkSMS",
        formatterParams: {
          precision: 0
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
    // Commented this out for the time being as it's proving rather cumbersome getting the
    // data display right on sort
    //pageLoaded: function (pageno) {
    //  SortFloatData(this);
    //}
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

function SortFloatData(tabulatorTableRef) {
  tabulatorTableRef.setSort([
    { column: "SaccoName", dir: "asc" }, //sort by this first
    { column: "MPesaFloat", dir: "asc" }, //then sort by this second
    //{ column: "BulkSMSFloat", dir: "asc" }, //then sort by this last
  ]);
}