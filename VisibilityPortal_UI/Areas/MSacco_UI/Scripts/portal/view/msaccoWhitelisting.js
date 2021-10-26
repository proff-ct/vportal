var tlrSingle = "SINGLE";
var tlrBatch = "BATCH";

var tblTabulator_Single;
var tblTabulator_Batch;

var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

var selectedMembersBatchFile;

//function initTabulator(tableContainerID, apiCommParams) {
function initTabulator(tlrModeSpec, apiCommParams) {
  //tlrModeSpec is an object { tlrName, tblID }
  switch (tlrModeSpec.tlrName) {
    case tlrSingle:
      //create Tabulator on DOM element with id == tableContainerID 
      tblTabulator_Single = new Tabulator(tlrModeSpec.tblID, {
        height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
        //data: tabledata, //assign data to table
        placeholder: "No Member Record(s) Found ",
        //pagination: "remote", //enable remote pagination
        //paginationSize: 100,
        //paginationSizeSelector: true,
        //ajaxProgressiveLoad: "scroll",
        //ajaxURL: 'Loans/GetLoanRecords',
        ajaxResponse: function (url, params, response) {
          //url - the URL of the request
          //params - the parameters passed with the request
          //response - the JSON object returned in the body of the response.

          response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
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
          { title: "Member Phone Number", field: "PhoneNumber" },
          //{ title: "Reason Blocked", field: "Comments" },
          {
            title: "MSACCO Registration Date", field: "DateRegistered",
            align: "center",
            formatter: function (cell, formatterParams) {
              return GetFormattedDate(cell.getValue());
            }
          }

        ],
        movableColumns: true

      });
      $(tblTabulator_Single.element).addClass("table table-striped table-condensed table-hover");
      break;
    case tlrBatch:
      //create Tabulator on DOM element with id == tableContainerID 
      tblTabulator_Batch = new Tabulator(tlrModeSpec.tblID, {
        height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
        //data: tabledata, //assign data to table
        placeholder: "No Member Record(s) Imported / Found ",
        //pagination: "remote", //enable remote pagination
        //paginationSize: 100,
        //paginationSizeSelector: true,
        //ajaxProgressiveLoad: "scroll",
        //ajaxURL: 'Loans/GetLoanRecords',
        //ajaxResponse: function (url, params, response) {
        //  //url - the URL of the request
        //  //params - the parameters passed with the request
        //  //response - the JSON object returned in the body of the response.

        //  response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
        //  return response;
        //},
        dataLoaded: function (data) {
          //data - all data loaded into the table. This is expected to be json data
          var loaded_rows = this.getRows();
          var memberRecord;

          loaded_rows.forEach(dataRow => {
            memberRecord = dataRow.getData();

            WhitelistMember(
              tlrModeSpec.restUrl,
              {
                client: tlrModeSpec.client,
                phoneNo: memberRecord.PhoneNumber,
                trustReason: memberRecord.KYC,
                processingMode: tlrBatch
              },
              apiCommParams,
              function (status, comments) {
                dataRow.update({ Status: status, Comments: comments });
              });
          });
          if (!selectedMembersBatchFile) return;
          bootbox.alert({
            title: "<h3>MSACCO Whitelisting - Batch File</h3>",
            message: loaded_rows.length + " record(s) imported"
          });
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
          { title: "Member Phone Number", field: "PhoneNumber", headerFilter: true },
          { title: "KYC Information", field: "KYC", formatter: "textarea", headerFilter: true },
          {
            title: "Status", field: "Status",
            headerFilter: "select", headerFilterFunc: "=",
            headerFilterParams: { values: true }
          },
          { title: "Comments", field: "Comments", formatter: "textarea", headerSort: false }
        ],
        movableColumns: true,
        index: "PhoneNumber",
      });
      $(tblTabulator_Batch.element).addClass("table table-striped table-condensed table-hover");
  }
}


function GetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function ClearFilters(tlrType) {
  switch (tlrType) {
    case tlrSingle:
      tblTabulator_Single.clearFilter(true);
      break;
    case tlrBatch:
      tblTabulator_Batch.clearFilter(true);
      break;
  }
}

function SearchMemberRecord(restUrl, corporateNo, phoneNo) {
  if (corporateNo == '' || corporateNo == undefined || phoneNo == '' || phoneNo == undefined) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    memberTelephoneNo: phoneNo
  };

  tabulatorAjaxUrlForReload = restUrl;
  tabulatorAjaxParamsForReload = ajaxParams

  ClearFilters(tblTabulator_Single);
  tblTabulator_Single.setData(restUrl, ajaxParams);
}

function ReloadData() {
  tblTabulator_Single.setData(tabulatorAjaxUrlForReload, tabulatorAjaxParamsForReload);
}

function WhitelistMember(restUrl, memberRecord, apiCommParams, batchCallback = null) {
  // memberRecord is object { client, phoneNo, trustReason, processingMode }
  if (memberRecord.client == '' || memberRecord.client == undefined || memberRecord.phoneNo == '' || memberRecord.phoneNo == undefined || memberRecord.trustReason == '' || memberRecord.trustReason == undefined || apiCommParams == undefined) return;
  var ajaxParams = {
    clientCorporateNo: memberRecord.client,
    memberTelephoneNo: memberRecord.phoneNo,
    trustReason: memberRecord.trustReason
  };

  $.post(restUrl, ajaxParams, function (response) {
    if (response == '') {
      response = { success: false, ex: "Server did not provide a response" };
    }
    else {
      try {
        response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
      }
      catch (err) {
        console.log(
          "Error on " + ajaxParams.memberTelephoneNo + " Failed interpreting response: " + err.message + " at " + moment().format("DD-MMM-YYYY hh:mm:ss A"));
        response = { success: false, ex: "An error occurred interpreting the server's response. Kindly try again" };
      }
    }
    switch (memberRecord.processingMode) {
      case tlrSingle:
        ParseSingleMemberWhitelistingResponse(response);
        break;
      case tlrBatch:
        ParseBatchWhitelistingResponse(response, batchCallback);
        break;
    }

  }).fail(function (error) {
    if (batchCallback) {
      var msg = { success: false, ex: "An error occurred communicating with MSACCO. Kindly try again" };
      ParseBatchWhitelistingResponse(msg, batchCallback);
    }
  });
}


function ParseSingleMemberWhitelistingResponse(serverResponse) {
  var msg;
  if (serverResponse.success == true) {
    msg = "<h4>Status: Success</h4>  <p/><p/>" + serverResponse.ex + "<p/><p/>Member can now transact on MSACCO"
  } else {
    msg = "<h4>Status: Failed</h4> <p/><p/>" + serverResponse.ex;
  }

  bootbox.alert({
    title: "<h3>MSACCO Whitelisting</h3>",
    message: msg
  });
}

function ParseBatchWhitelistingResponse(serverResponse, batchCallback) {
  if (serverResponse.success === true) {
    batchCallback("Success", serverResponse.ex + " Member can now transact on MSACCO");
  } else {
    batchCallback("Failed", serverResponse.ex);
  }
}


function initResponsiveTabs(tabsContainerRef) {
  $(tabsContainerRef).responsiveTabs({
    startCollapsed: 'accordion'
  });
}

function ImportMemberRecordFromFile(evtLoadFile) {
  const wsRef = "!ref";

  var data = evtLoadFile.target.result;
  var recordsToWhitelist = [];

  var workbook = XLSX.read(data, {
    type: "binary"
  });

  var ws;
  var range;

  workbook.SheetNames.forEach(sheet => {
    // verify sheet has reader row
    range = null;
    ws = workbook.Sheets[sheet];

    if (!ws.hasOwnProperty(wsRef)) return;
    range = XLSX.utils.decode_range(ws[wsRef]);

    if (!IsHeaderRowPresent(ws)) return;

    var whitelistData = {
      Phone: null,
      KYC: null
    };
    var IDX_LAST_COL = 1;

    for (var R = 1; R <= range.e.r; ++R) {
      for (var C = 0; C <= IDX_LAST_COL; ++C) {

        var cell_ref = XLSX.utils.encode_cell({ c: C, r: R });
        var cell_value = ExtractCellData(ws[cell_ref]);
        if (!cell_value) break;

        switch (C) {
          case 0:
            whitelistData.Phone = cell_value;
            break;
          case 1:
            whitelistData.KYC = cell_value;
            break;
        }
        if (C === IDX_LAST_COL) {
          recordsToWhitelist.push({
            PhoneNumber: whitelistData.Phone,
            KYC: whitelistData.KYC,
            Status: "Pending",
            Comments: null
          });
        }

      }

    }

    LoadMemberRecordsToTabulator(JSON.stringify(recordsToWhitelist));
  });

  if (selectedMembersBatchFile && recordsToWhitelist.length < 1) {
    bootbox.alert({
      title: "<h3>MSACCO Whitelisting</h3>",
      message: "No records found in " + selectedMembersBatchFile.name + " to import"
    });
  }

}

function LoadMemberRecordsToTabulator(parsedMemberRecords) {
  tblTabulator_Batch.setData(parsedMemberRecords)
    .then(function (data) {
      //run code after table has been successfuly updated
    })
    .catch(function (error) {
      //handle error loading data
      bootbox.alert({
        title: "<h3>MSACCO Whitelisting</h3>",
        message: "Error loading data from " + selectedMembersBatchFile.name + ". Kindly try again or select a different file."
      });
    });
}

function DownloadBatchFileTemplate() {
  var wb = XLSX.utils.book_new();
  var templateSheetName = "Numbers to Whitelist";
  var excelFileName = "MSACCO_BATCH_FILE_WHITELISTING_TEMPLATE.xlsx";

  wb.Props = {
    Title: "MSACCO Whitelisting Batch File",
    Subject: "Member Records Template",
    Author: "CoreTec MSACCO",
    CreatedDate: moment().format("DD-MMM-YYYY_HHmmss")
  };

  wb.SheetNames.push(templateSheetName);
  var ws_data = [['Phone Number (254xxxxxxxxx)', 'KYC Information']];  //a row with 2 columns
  var ws = XLSX.utils.aoa_to_sheet(ws_data);
  wb.Sheets[templateSheetName] = ws;
  XLSX.writeFile(wb, excelFileName, { bookType: 'xlsx', type: 'binary' });

  //utilize Filesaver.js and Blob to handle the file saving for cross browser support.
  //var wbToFile = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
  //saveAs(new Blob([s2ab(wbToFile)], { type: "application/octet-stream" }), excelFileName);
}

function s2ab(s) {
  var buf = new ArrayBuffer(s.length); //convert s to arrayBuffer
  var view = new Uint8Array(buf);  //create uint8array as viewer
  for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF; //convert to octet
  return buf;
}

function IsHeaderRowPresent(workSheet) {
  var phoneNumber = workSheet[XLSX.utils.encode_cell({ c: 0, r: 0 })];
  var kyc = workSheet[XLSX.utils.encode_cell({ c: 1, r: 0 })];

  return phoneNumber !== undefined &&
    kyc !== undefined &&
    DoesStringContainSubString(phoneNumber.v, "Phone Number") &&
    DoesStringContainSubString(kyc.v, "KYC Information");
}

function ExtractCellData(sheetjs_cell) {
  return sheetjs_cell ? sheetjs_cell.v : null;
}
