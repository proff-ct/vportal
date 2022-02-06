var selectedSMSContactsFile;
var parsedBulkSMSFile = {
    FileName: null,
    Message: null,
    RecipientList: null
};

var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, apiCommParams) {
    //create Tabulator on DOM element with id == tableContainerID
    tblTabulator = new Tabulator(tableContainerID, {
        height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
        //data: tabledata, //assign data to table
        placeholder: "No SMS Records Found ",
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
            { title: "No", field: "FileNo", headerFilter: true },
            {
                title: "Status", field: "Status",
                headerFilter: "select", headerFilterFunc: "=",
                headerFilterParams: { values: true }
            },
            //{ title: "Message", field: "Message", headerFilter: true },
            { title: "File Name", field: "SMSFileName", headerFilter: true },
            {
                title: "Date Submitted", field: "DateDispatched",
                align: "center", headerFilter: true,
                formatter: function (cell, formatterParams) {
                    return GetFormattedDate(cell.getValue());
                }
            },
            { title: "Submitted By", field: "ActionUser", headerFilter: true },

            //{ title: "Session ID", field: "SESSION_ID", headerFilter: true },
            //{ title: "Corporate No", field: "Corporate_No" },
        ],
        movableColumns: true,
        index: "FileNo",
        initialSort: [
            { column: "FileNo", dir: "desc" }
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

function InitParsedContactFile() {
    parsedBulkSMSFile = {
        FileName: null,
        Message: null,
        RecipientList: null
    };
}

function LoadRecipientData(evtLoadFile) {
    const wsRef = "!ref";

    var data = evtLoadFile.target.result;
    var contactList = [];

    var workbook = XLSX.read(data, {
        type: "binary"
    });

    var ws;
    var range;

    var numContacts = 0;
    InitParsedContactFile();

    workbook.SheetNames.forEach(sheet => {
        // verify sheet has reader row
        range = null;
        ws = workbook.Sheets[sheet];

        if (!ws.hasOwnProperty(wsRef)) return;
        range = XLSX.utils.decode_range(ws[wsRef]);

        // check line headers present. return if not present
        if (!IsHeaderRowPresent(ws)) {
            PopupMessage("Bulk SMS", "Invalid File Header<p><p><em>Cell A1 should have value: Phone No</em>");
            return;
        }

        var IDX_FIRST_DATA_ROW = 1;
        var IDX_DATA_COL = 0;
        numContacts = 0;

        for (var R = IDX_FIRST_DATA_ROW; R <= range.e.r; ++R) {

            var cell_ref = XLSX.utils.encode_cell({ c: IDX_DATA_COL, r: R });
            var cell_value = ExtractCellData(ws[cell_ref]);
            if (!cell_value) break;

            contactList.push(cell_value);
            numContacts++;
        }

        parsedBulkSMSFile.FileName = selectedSMSContactsFile.name;
        parsedBulkSMSFile.RecipientList = contactList;

        // exit on sheet 1
        return;
    });

    bootbox.alert({
        title: "<h4>Bulk SMS</h4> - Load Contacts",
        message: "Imported " + numContacts + " contacts"
    });
}

function IsHeaderRowPresent(workSheet) {
    var IDX_HEADER_ROW = 0;
    var IDX_HEADER_COL = 0;

    var header = workSheet[XLSX.utils.encode_cell({ c: IDX_HEADER_COL, r: IDX_HEADER_ROW })];
    return header !== undefined && DoesStringContainSubString(header.v, "Phone No");
}

function ExtractCellData(sheetjs_cell) {
    return sheetjs_cell ? sheetjs_cell.v : null;
}

function DispatchSMS(restUrl, parsedSMS, apiCommParams, successCallBack = null) {
    if (parsedSMS.FileName == null || parsedSMS.FileName === undefined || parsedSMS.Message == null || parsedSMS.Message === undefined || parsedSMS.RecipientList == null || parsedSMS.RecipientList === undefined) {

        PopupMessage("<h5>Bulk SMS</h5>","Dispatch failed<br><br>Invalid data");
        return;
    }

    var ajaxParams = {
        FileName: parsedSMS.FileName,
        Message: parsedSMS.Message,
        RecipientList: parsedSMS.RecipientList
    };
    apiCommParams.requestType = requestType.POST;

    msaccoCallBack.SUCCESS = function (response) {
        if (response == '') {
            response = { success: false, ex: "Server did not provide a response" };
        }
        else {
            try {
                response = JSON.parse(MSACCODecryptor(apiCommParams.encSecret, apiCommParams.encKey, response));
            }
            catch (err) {
                console.log(
                    "Error on " + ajaxParams.statementFile.StatementFileName + " Failed interpreting response: " + err.message + " at " + moment().format(PORTAL_DATE_FORMAT));
                response = { success: false, ex: "An error occurred interpreting the server's response. Kindly try again" };
            }
        }
        ParseSMSDispatchResponse(response, successCallBack);
    };

    msaccoCallBack.ERROR = function (xhr, status, error) {
        var msg = { success: false, ex: null };
        var errorCode = xhr.status;

        if (errorCode == ERR_CODE.BAD_REQ) {
            if (error) {
                msg.ex = "MSACCO says: " + error + "<p/><p/>Kindly log out then log back in to resolve this error";
            }
            else msg.ex = "MSACCO returned an error<p/><p/>Kindly log out then log back in to resolve.";
        }
        else if (errorCode == 200 && status == "parsererror") {
            msg.ex = "Server error. <p/><p/>Close all portal tabs then log out and log back in.";
        }
        else msg.ex = "An error occurred communicating with MSACCO. Kindly try again";

        ParseSMSDispatchResponse(msg);
    };


    CallMSACCO(restUrl, ajaxParams, apiCommParams, msaccoCallBack);
}

function ParseSMSDispatchResponse(serverResponse, successCallBack) {
    InitParsedContactFile();
    var msg;
    if (serverResponse.success == true) {
        msg = "<h4>Status: Success</h4>  <p/><p/>" + serverResponse.ex;
        if(successCallBack) successCallBack();
    } else {
        msg = "<h4>Status: Failed</h4> <p/><p/>" + serverResponse.ex;
    }

    bootbox.alert({
        title: "<h4>Bulk SMS</h4>",
        message: msg
    });
}