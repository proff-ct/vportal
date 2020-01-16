// the values of the datasets must match the array members of the var datasets in index.cshtml
var datasets = {
  loan: 'Loans',
  withdrawal: 'Withdrawals',
  utilityPayment: 'UtilityPayments',
  airtimeTopUp: 'AirtimeTopups'
}

var arrayOfDatasetTables = [];

function DisplayDateNow(dateFormat = null) {
  return moment().format((dateFormat == null) ? "DD-MMM-YYYY HH:mm:ss" : dateFormat)
}

function DisplayDefaultQuantity(defaultQty = null) {
  return defaultQty || "---"
}

function initTabulator(datasetConfig) {
  var datasetTable = {};
  switch (datasetConfig.datasetName) {
    case datasets.loan:
      datasetTable[datasets.loan] = new LoansTable(datasetConfig.containerRef);
      break;
    case datasets.withdrawal:
      datasetTable[datasets.withdrawal] = new WithdrawalsTable(datasetConfig.containerRef);
      break;
    case datasets.utilityPayment:
      datasetTable[datasets.utilityPayment] = new UtilityPaymentsTable(datasetConfig.containerRef);
      break;
    case datasets.airtimeTopUp:
      datasetTable[datasets.airtimeTopUp] = new AirtimeTopUpsTable(datasetConfig.containerRef);
      break;

  }
  arrayOfDatasetTables.push(datasetTable);
}

function ReloadData(datasetName) {
  // get the object that matches the supplied ds name
  var dsTable = arrayOfDatasetTables.find(table => table.hasOwnProperty(datasetName));

  // invoke the ReloadData function on that object
  dsTable[datasetName].ReloadData();
}

function ClearFilters(datasetName) {
  var dsTable = arrayOfDatasetTables.find(table => table.hasOwnProperty(datasetName));

  dsTable[datasetName].ClearFilters();
  dsTable[datasetName].ReloadData();
}

class LoansTable {
  tblTabulator = null;
  tabulatorAjaxUrlForReload = null;
  tabulatorAjaxParamsForReload = null;

  constructor(htmlContainerRef) {
    var boundHideGuarantorSubTables = this.HideGuarantorSubTables.bind(this);
    this.tblTabulator = new Tabulator(htmlContainerRef, {
      height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
      //data: tabledata, //assign data to table
      placeholder: "No Loan Records Found ",
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
          headerFilter: true
        },
        {
          title: "Trx Date", field: "Transaction_Date",
          align: "center", headerFilter: true,
          formatter: function (cell, formatterParams) {
            return GetFormattedDate(cell.getValue());
          }
        },
        { title: "Disbursed ?", field: "Disbursed" },
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
        //HideGuarantorSubTables();
        boundHideGuarantorSubTables
      },
      pageLoaded: function (pageno) {
        //pageno - the number of the loaded page
        //alert(pageno)
        //HideGuarantorSubTables();
        boundHideGuarantorSubTables
      }
    });
    this.ClearFilters = tabulatorClearFilters.bind(this.tblTabulator);
    $(this.tblTabulator.element).addClass("table table-striped table-condensed table-hover");
  }

  initGuarantorSubTable(tableElement, tableData) {
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

  HideGuarantorSubTables() {
    // how do I toggle visibility?
    // - get the visibile rows
    // - get the row id's
    // - for each id, hide the subtable
    // when do I toggle visibility?
    // - when the page finishes rendering
    var pageRows = this.tblTabulator.getRows();
    pageRows.forEach(function (pageRow) {
      // get reference to child table element and hide
      $(pageRow.getElement()).children("div.subTable" + pageRow.getIndex()).toggle(false);
    });
  }

  LoadData(restUrl, corporateNo) {
    if (corporateNo == '' || corporateNo == undefined) return;
    var ajaxParams = { clientCorporateNo: corporateNo };

    this.tabulatorAjaxUrlForReload = restUrl;
    this.tabulatorAjaxParamsForReload = ajaxParams

    this.ClearFilters();
    this.tblTabulator.setData(restUrl, ajaxParams);
  }

  ClearFilters() {
  }

  ReloadData() {
    this.tblTabulator.setData(this.tabulatorAjaxUrlForReload, this.tabulatorAjaxParamsForReload);
  }
}
class WithdrawalsTable {
  tblTabulator = null;
  tabulatorAjaxUrlForReload = null;
  tabulatorAjaxParamsForReload = null;

  constructor(htmlContainerRef) {
    var boundGetFormattedDate = this.GetFormattedDate.bind(this);
    this.tblTabulator = new Tabulator(htmlContainerRef, {
      height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
      //data: tabledata, //assign data to table
      placeholder: "No Mobile Withdrawal Records Found ",
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
        { title: "Telephone No", field: "TelephoneNo", headerFilter: true },
        {
          title: "Comments", field: "Comments",
          headerFilter: "select", headerFilterFunc: "=",
          headerFilterParams: { values: true }
        },
        { title: "Account Name", field: "Account_Name", headerFilter: true },
        {
          title: "Trx Date", field: "Transaction_Date",
          align: "center", headerFilter: true,
          formatter: function (cell, formatterParams) {
            return boundGetFormattedDate(cell.getValue());
          }
        },
        { title: "Amount", field: "Amount", headerFilter: true },
        { title: "Account Balance", field: "Account_Balance", headerFilter: true },
        { title: "Request Confirmed", field: "RequestConfirmed", headerFilter: true },
        { title: "Account No", field: "Account_No", headerFilter: true },

        { title: "MPESA Result Type", field: "MPESA_Result_Type", headerFilter: true },
        { title: "MPESA Result Desc", field: "MPESA_Result_Desc", headerFilter: true },
        {
          title: "MPESA DateTime", field: "MPESA_DateTime",
          align: "center", headerFilter: true,
          formatter: function (cell, formatterParams) {
            return boundGetFormattedDate(cell.getValue());
          }
        },
        { title: "MPESA Float Amount", field: "MPESA_Float_Amount", headerFilter: true },
        { title: "Document No", field: "Document_No", headerFilter: true },
        { title: "MPesa Name", field: "MPesa_Name", headerFilter: true },
        {
          title: "Sent to Journal?", field: "Sent_To_Journal",
          headerFilter: "select", headerFilterFunc: "=",
          headerFilterParams: { values: true }
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
    this.ClearFilters = tabulatorClearFilters.bind(this.tblTabulator);
    $(this.tblTabulator.element).addClass("table table-striped table-condensed table-hover");

  }

  GetFormattedDate(objDate) {
    return (objDate == null) ?
      'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
  }

  ClearFilters() {
  }

  LoadData(restUrl, corporateNo) {
    if (corporateNo == '' || corporateNo == undefined) return;
    var ajaxParams = { clientCorporateNo: corporateNo };

    this.tabulatorAjaxUrlForReload = restUrl;
    this.tabulatorAjaxParamsForReload = ajaxParams

    this.ClearFilters();
    this.tblTabulator.setData(restUrl, ajaxParams);
  }

  ReloadData() {
    this.tblTabulator.setData(this.tabulatorAjaxUrlForReload, this.tabulatorAjaxParamsForReload);
  }

}
class UtilityPaymentsTable {
  tblTabulator = null;
  tabulatorAjaxUrlForReload = null;
  tabulatorAjaxParamsForReload = null;

  constructor(htmlContainerRef) {
    var boundGetFormattedDate = this.GetFormattedDate.bind(this);
    this.tblTabulator = new Tabulator(htmlContainerRef, {
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
            return boundGetFormattedDate(cell.getValue());
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
    this.ClearFilters = tabulatorClearFilters.bind(this.tblTabulator);
    $(this.tblTabulator.element).addClass("table table-striped table-condensed table-hover");
  }

  GetFormattedDate(objDate) {
    return (objDate == null) ?
      'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
  }

  ClearFilters() { }

  LoadData(restUrl, corporateNo) {
    if (corporateNo == '' || corporateNo == undefined) return;
    var ajaxParams = { clientCorporateNo: corporateNo };

    this.tabulatorAjaxUrlForReload = restUrl;
    this.tabulatorAjaxParamsForReload = ajaxParams

    this.ClearFilters();
    this.tblTabulator.setData(restUrl, ajaxParams);
  }

  ReloadData() {
    this.tblTabulator.setData(this.tabulatorAjaxUrlForReload, this.tabulatorAjaxParamsForReload);
  }

}
class AirtimeTopUpsTable {
  tblTabulator = null;
  tabulatorAjaxUrlForReload = null;
  tabulatorAjaxParamsForReload = null;

  constructor(htmlContainerRef) {
    var boundGetFormattedDate = this.GetFormattedDate.bind(this);
    this.tblTabulator = new Tabulator(htmlContainerRef, {
        height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
        //data: tabledata, //assign data to table
        placeholder: "No Airtime Topup Records Found ",
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
          { title: "Telephone No", field: "TelephoneNo", headerFilter: true },
          {
            title: "Comments", field: "Comments",
            headerFilter: "select", headerFilterFunc: "=",
            headerFilterParams: { values: true }
          },
          { title: "Account Name", field: "Account_Name", headerFilter: true },
          {
            title: "Airtime Received", field: "Airtime_Received",
            headerFilter: "select", headerFilterFunc: "=",
            headerFilterParams: { values: true }
          },
          {
            title: "Trx Date", field: "Transaction_Date",
            align: "center", headerFilter: true,
            formatter: function (cell, formatterParams) {
              return boundGetFormattedDate(cell.getValue());
            }
          },
          { title: "Utility Payment Account", field: "Utility_Payment_Account", headerFilter: true },
          { title: "Amount", field: "Amount", headerFilter: true },
          { title: "Account Balance", field: "Account_Balance", headerFilter: true },
          { title: "Request Confirmed", field: "RequestConfirmed", headerFilter: true },
          { title: "Account No", field: "Account_No", headerFilter: true },

          { title: "Airtime Result Type", field: "Airtime_Result_Type", headerFilter: true },
          { title: "Document No", field: "Document_No", headerFilter: true },
          { title: "from MSISDN", field: "From_MSISDN", headerFilter: true },
          {
            title: "Sent to Journal?", field: "Sent_To_Journal",
            headerFilter: "select", headerFilterFunc: "=",
            headerFilterParams: { values: true }
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
    this.ClearFilters = tabulatorClearFilters.bind(this.tblTabulator);
    $(this.tblTabulator.element).addClass("table table-striped table-condensed table-hover");
    
  }

GetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

ClearFilters() {}

LoadData(restUrl, corporateNo) {
  if (corporateNo == '' || corporateNo == undefined) return;
  var ajaxParams = { clientCorporateNo: corporateNo };

  this.tabulatorAjaxUrlForReload = restUrl;
  this.tabulatorAjaxParamsForReload = ajaxParams

  this.ClearFilters();
  this.tblTabulator.setData(restUrl, ajaxParams);
}

ReloadData() {
  this.tblTabulator.setData(this.tabulatorAjaxUrlForReload, this.tabulatorAjaxParamsForReload);
}

}