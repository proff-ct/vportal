var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, apiCommParams) {
  var dateFieldAccessor = function (value, data, type, params, column) {
    return GetFormattedDate(value);
  };

  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Loan Records Found ",
    pagination: "remote", //enable remote pagination
    paginationSize: 100,
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
      { title: "Amount", field: "Amount", headerFilter: true },
      //{ title: "Session ID", field: "SESSION_ID", headerFilter: true },
      {
        title: "Sent to Journal?", field: "Sent_To_Journal",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      //{ title: "Corporate No", field: "Corporate_No" },
      //{ title: "Type", field: "Type" },
      {
        title: "Loan Type", field: "Loan_type",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Guarantors", field: "No_of_Guarantors",
        headerFilter: true, download: false
      },
      {
        title: "Trx Date", field: "Transaction_Date", downloadTitle:"Transaction Date",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        },
        accessorDownload: dateFieldAccessor,
        accessorDownloadParams: {}
      },
      { title: "Disbursed ?", field: "Disbursed", download:false },
    ],
    movableColumns: true,
    index: "Entry_No",
    initialSort: [
      { column: "Entry_No", dir: "desc" }
    ],
    headerSortTristate: true,
    rowClick: function (e, row) {
      // toggle visibility of the guarantor info
      // if child has triggered event, stop propagation else allow
      // how will i know it is the child that has triggered?
      // - get reference to the dom element that is the event target
      // - use reference to check if any of the parents has child id
      // - if match found, then we know it's the child we are dealing with
      var targetEl = $(e.target);
      const id = row.getIndex();
      if (targetEl.parents('div.subTable' + id).first().length) {
        // IS CHILD. DON'T TOGGLE!
        // this is so that we can have the normal sorting behaviour
        // occur on the sub table
        e.stopImmediatePropagation();

      } else {
        $(".subTable" + id + "").toggle();
      }

    },
    rowFormatter: function (row, e) {
      const id = row.getIndex();

      //create and style holder elements
      var holderEl = document.createElement("div");
      var tableEl = document.createElement("div");

      holderEl.style.boxSizing = "border-box";
      holderEl.style.padding = "10px 30px 10px 10px";
      holderEl.style.borderTop = "1px solid #333";
      holderEl.style.borderBotom = "1px solid #333";
      holderEl.style.background = "#ddd";
      holderEl.setAttribute('class', "subTable" + id + "");

      tableEl.style.border = "1px solid #333";
      //tableEl.setAttribute('class', "subTable" + id + "");

      holderEl.appendChild(tableEl);

      row.getElement().appendChild(holderEl);

      initGuarantorSubTable(tableEl, row.getData().Guarantors);
    },
    renderComplete: function () {
      // make guarantor subtables invisible by default
      HideGuarantorSubTables();
    },
    pageLoaded: function (pageno) {
      //pageno - the number of the loaded page
      //alert(pageno)
      HideGuarantorSubTables();
    }
  });
  $(tblTabulator.element).addClass("table table-striped table-condensed table-hover");
}

function initGuarantorSubTable(tableElement, tableData) {
  var subTable = new Tabulator(tableElement, {
    layout: "fitDataFill",
    data: tableData,
    placeholder: "No Guarantor Data ",
    responsiveLayout: "collapse",
    columns: [{
      formatter: "responsiveCollapse", width: 30, minWidth: 30,
      align: "center", resizable: false, headerSort: false
    },
    {
      title: "Record ID",
      field: "Id"
    },
    {
      title: "Guarantor",
      field: "Guarantor"
    },
    {
      title: "Amount",
      field: "Amount"
    },
    {
      title: "Status",
      field: "Status"
    },
    {
      title: "Timestamp",
      field: "Datetime",
      //sorter: "date",
      formatter: function (cell, formatterParams) {
        return GetFormattedDate(cell.getValue());
      }
    },
    {
      title: "Sent SMS?",
      field: "Sent_Sms"
    },
    {
      title: "Posted?",
      field: "Posted"
    }
    ]
  })
}

function GetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function HideGuarantorSubTables() {
  // how do I toggle visibility?
  // - get the visibile rows
  // - get the row id's
  // - for each id, hide the subtable
  // when do I toggle visibility?
  // - when the page finishes rendering
  var pageRows = tblTabulator.getRows();
  pageRows.forEach(function (pageRow) {
    // get reference to child table element and hide
    $(pageRow.getElement()).children("div.subTable" + pageRow.getIndex()).toggle(false);
  });
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
  var excelFileName = "MSACCO_Loans_" + moment().format("DD-MMM-YYYY_HHmmss") + ".xlsx";
  tblTabulator.download("xlsx", excelFileName, {
    sheetName: "Loan Records"
  });
}