﻿const SACCO_VITALS = {
  Floats: {
    MPESA: "MPESA",
    BULK_SMS: "BULK_SMS",
    BANK_TRANSFER: "BANK_TRANSFER"
  },
  LinkStatus: "LinkStatus"
};
const CALLBACK_RESPONSE = {
  STATUS: {
    OK: "OK",
    ERROR: "ERR"
  }
};

// the values of the datasets must match the array members of the var datasets in index.cshtml
var datasets = {
  loan: 'Loans',
  withdrawal: 'Withdrawals',
  utilityPayment: 'UtilityPayments',
  airtimeTopUp: 'AirtimeTopups'
}

var arrayOfDatasetTables = [];
var arrayOfDataSetFiscalSummary = [];

function DisplayDateNow(date =null, dateFormat = null) {
  return (!date) ? "No date" : moment(date).format((dateFormat == null) ? "DD-MMM-YYYY HH:mm:ss" : dateFormat);
}

function DisplayDefaultQuantity(defaultQty = null) {
  return defaultQty || "---"
}


function LoadSaccoVitals(saccoVitals, callBack, vitalToLoad = null) {
  if (saccoVitals.corporateNo == "" || saccoVitals.corporateNo == null) return;
  var vitalRestUrl = null;
  var vitalLabel = null;

  if (vitalToLoad) {
    switch (vitalToLoad) {
      case SACCO_VITALS.Floats.MPESA:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.mpesaFloat.restUrl;
        break;
      case SACCO_VITALS.Floats.BULK_SMS:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.bulkSMSFloat.restUrl;
        break;
      case SACCO_VITALS.Floats.BANK_TRANSFER:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.bankTransferFloat.restUrl;
        break;
      case SACCO_VITALS.LinkStatus:
        vitalLabel = vitalToLoad;
        vitalRestUrl = saccoVitals.linkStatus.restUrl;
        break;
    }
    GetSaccoVitalsData(vitalRestUrl, saccoVitals.corporateNo, vitalLabel, callBack);
  } else {
    // no specific vital passed in from front end so load everything
    for (key in SACCO_VITALS) {
      switch (key) {
        case "Floats":
          for (float in SACCO_VITALS.Floats) {
            vitalLabel = float;
            switch (float) {
              case SACCO_VITALS.Floats.MPESA:
                vitalRestUrl = saccoVitals.mpesaFloat.restUrl;
                break;
              case SACCO_VITALS.Floats.BULK_SMS:
                vitalRestUrl = saccoVitals.bulkSMSFloat.restUrl;
                break;
              case SACCO_VITALS.Floats.BANK_TRANSFER:
                vitalRestUrl = saccoVitals.bankTransferFloat.restUrl;
                break;
            };
            GetSaccoVitalsData(vitalRestUrl, saccoVitals.corporateNo, vitalLabel, callBack);
          };
          break;
        case "LinkStatus":
          vitalLabel = SACCO_VITALS.LinkStatus;
          vitalRestUrl = saccoVitals.linkStatus.restUrl;

          GetSaccoVitalsData(vitalRestUrl, saccoVitals.corporateNo, vitalLabel, callBack);
          break;
      }
    };
  }
}

function GetSaccoVitalsData(restUrl, corporateNo, vitalToGet, callBack) {
  $.ajax({
    method: "GET",
    //dataType: 'json',
    url: restUrl,
    data: {
      clientCorporateNo: corporateNo
    },
    success: function (responseData) {
      var response = {};
      if (responseData === "" || responseData === null) {
        response = {
          status: CALLBACK_RESPONSE.STATUS.ERROR,
          error: "No data"
        };
      }
      else {
        response = {
          status: CALLBACK_RESPONSE.STATUS.OK,
          data: {
            amount: responseData.amount,
            last_transaction_timestamp: responseData.last_transaction_timestamp,
          }
        };
      }
      callBack(vitalToGet, response);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      var response = {
        status: CALLBACK_RESPONSE.STATUS.ERROR,
        error: $(xhr.responseText).filter('title').get(0).text
      };
      callBack(vitalToGet, response);
    }
  });
}

function DisplayFinancialSummaryForTheDay(datasetParams)
{
  $.ajax({
    type: "GET",
    url: datasetParams.restUrlForFiscalSummary,
    data: { clientCorporateNo: datasetParams.corporateNo},
    success: function (data) {
      //call is successfully completed and we got result in data
      if (data == '') return;
      last_transaction_timestamp = data.last_transaction_timestamp;
      totalSum = data.sum;

      $(datasetParams.datePlaceholderRef).html(DisplayDateNow(data.last_transaction_timestamp));
      //$(datasetParams.quantityPlaceholderRef).html("KES "+data.sum);
      $(datasetParams.quantityPlaceholderRef).html(FormatAsCurrency(data.sum));
    },

    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      //alert("Server Error Occurred. Unable to load summary");
      //bootbox.alert({
      //  title: "<h3>Daily Financial Total</h3>",
      //  message: "Server Error Occurred. Unable to load " + datasetParams.datasetName + " summary." 
      //})
      //Log(xhr.responseText);
    }
  });
}

function RefreshFiscalSummary(ds) {
  DisplayFinancialSummaryForTheDay(arrayOfDataSetFiscalSummary[ds]);
}


function initTabulator(datasetConfig, apiCommParams) {
  var datasetTable = {};
  switch (datasetConfig.datasetName) {
    case datasets.loan:
      datasetTable[datasets.loan] = new LoansTable(datasetConfig.containerRef, apiCommParams);
      break;
    case datasets.withdrawal:
      datasetTable[datasets.withdrawal] = new WithdrawalsTable(datasetConfig.containerRef, apiCommParams);
      break;
    case datasets.utilityPayment:
      datasetTable[datasets.utilityPayment] = new UtilityPaymentsTable(datasetConfig.containerRef, apiCommParams);
      break;
    case datasets.airtimeTopUp:
      datasetTable[datasets.airtimeTopUp] = new AirtimeTopUpsTable(datasetConfig.containerRef, apiCommParams);
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

function FormatAsCurrency(numToFormat = null) {
  var defaultCurrencySymbol = "KES";
  var defaultCurrencyFormat = defaultCurrencySymbol + ' 0,0[.]00';


  numeral.nullFormat('0');
  return defaultCurrencySymbol + ' '+ numeral(numToFormat).format(defaultCurrencyFormat);
}

function FormatSMSFloat(smsFloat) {
  var floatFormat = '(0,0)';

  numeral.nullFormat('---');
  return numeral(smsFloat).format(floatFormat);
}

class LoansTable {
  tblTabulator = null;
  tabulatorAjaxUrlForReload = null;
  tabulatorAjaxParamsForReload = null;

  constructor(htmlContainerRef, apiCommParams) {
    var boundInitGuarantorSubTable = this.initGuarantorSubTable.bind(this);
    var boundHideGuarantorSubTables = this.HideGuarantorSubTables.bind(this);
    var boundGetFormattedDate = this.GetFormattedDate.bind(this);
    this.tblTabulator = new Tabulator(htmlContainerRef, {
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

        response.data = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response.data));
        return response; //return the tableData property of a response json object
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
          headerFilter: true
        },
        {
          title: "Trx Date", field: "Transaction_Date",
          align: "center", headerFilter: true,
          formatter: function (cell, formatterParams) {
            return boundGetFormattedDate(cell.getValue());
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

        boundInitGuarantorSubTable(tableEl, row.getData().Guarantors);
      },
      renderComplete: function () {
        // make guarantor subtables invisible by default
        boundHideGuarantorSubTables();
      },
      pageLoaded: function (pageno) {
        //pageno - the number of the loaded page
        //alert(pageno)
        boundHideGuarantorSubTables();
      }
    });
    this.ClearFilters = tabulatorClearFilters.bind(this.tblTabulator);
    $(this.tblTabulator.element).addClass("table table-striped table-condensed table-hover");
  }

  initGuarantorSubTable(tableElement, tableData) {
    var boundGetFormattedDate = this.GetFormattedDate.bind(this);
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
          return boundGetFormattedDate(cell.getValue());
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

  GetFormattedDate(objDate) {
    return (objDate == null) ?
      'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
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

  constructor(htmlContainerRef, apiCommParams) {
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
      ajaxResponse: function (url, params, response) {
        //url - the URL of the request
        //params - the parameters passed with the request
        //response - the JSON object returned in the body of the response.
        response.data = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response.data));
        return response; //return the tableData property of a response json object
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

  constructor(htmlContainerRef, apiCommParams) {
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
      ajaxResponse: function (url, params, response) {
        //url - the URL of the request
        //params - the parameters passed with the request
        //response - the JSON object returned in the body of the response.
        response.data = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response.data));
        return response; //return the tableData property of a response json object
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

  constructor(htmlContainerRef, apiCommParams) {
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
        ajaxResponse: function (url, params, response) {
          //url - the URL of the request
          //params - the parameters passed with the request
          //response - the JSON object returned in the body of the response.
          response.data = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response.data));
          return response; //return the tableData property of a response json object
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