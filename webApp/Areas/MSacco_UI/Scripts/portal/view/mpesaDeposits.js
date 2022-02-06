var selectedStatementFile;
var parsedStatementFile = {
    METADATA: null,
    Transactions: null
};

var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, apiCommParams) {
    //create Tabulator on DOM element with id == tableContainerID
    tblTabulator = new Tabulator(tableContainerID, {
        height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
        //data: tabledata, //assign data to table
        placeholder: "No Uploaded Deposit Records Found ",
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
                    return GetFormattedDate(cell.getValue());
                }
            },
            { title: "Amount", field: "Amount", headerFilter: true },
            { title: "Account Balance", field: "Account_Balance", headerFilter: true },
            //{ title: "Request Confirmed", field: "RequestConfirmed", headerFilter: true },
            { title: "Account No", field: "Account_No", headerFilter: true },
            { title: "MPESA Result Type", field: "MPESA_Result_Type", headerFilter: true },
            { title: "MPESA Result Desc", field: "MPESA_Result_Desc", headerFilter: true },
            { title: "MPESA Receipt #", field: "MPESA_Receipt_No", headerFilter: true },
            {
                title: "MPESA DateTime", field: "MPESA_DateTime",
                align: "center", headerFilter: true,
                formatter: function (cell, formatterParams) {
                    return GetFormattedDate(cell.getValue());
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



function LoadStatementData(evtLoadFile) {
    const wsRef = "!ref";

    var data = evtLoadFile.target.result;
    var statementLines = [];

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

        // get file data. return if no data
        var statementFile = ExtractFileMetaData(ws);
        if (statementFile == null) return;

        // check line headers present. return if not present
        if (!IsLineHeaderRowPresent(ws)) return;

        var lineData = {
            ReceiptNo: null,
            CompletionTime: null,
            InitiationTime: null,
            Details: null,
            TransactionStatus: null,
            PaidIn: null,
            Withdrawn: null,
            Balance: null,
            BalanceConfirmed: null,
            ReasonType: null,
            OtherPartyInfo: null,
            LinkedTransactionID: null,
            AccNo: null
        };

        var IDX_FIRST_LINE_ROW = 6;
        var IDX_LAST_LINE_COL = 12;

        for (var R = IDX_FIRST_LINE_ROW; R <= range.e.r; ++R) {
            for (var C = 0; C <= IDX_LAST_LINE_COL; ++C) {

                var cell_ref = XLSX.utils.encode_cell({ c: C, r: R });
                var cell_value = ExtractCellData(ws[cell_ref]);
                switch (C) {
                    case 0:
                        lineData.ReceiptNo = cell_value;
                        break;
                    case 1:
                        lineData.CompletionTime = moment(cell_value).format(PORTAL_DATE_FORMAT);
                        break;
                    case 2:
                        lineData.InitiationTime = moment(cell_value).format(PORTAL_DATE_FORMAT);
                        break;
                    case 3:
                        lineData.Details = cell_value;
                        break;
                    case 4:
                        lineData.TransactionStatus = cell_value;
                        break;
                    case 5:
                        lineData.PaidIn = cell_value;
                        break;
                    case 6:
                        lineData.Withdrawn = cell_value;
                        break;
                    case 7:
                        lineData.Balance = cell_value;
                        break;
                    case 8:
                        lineData.BalanceConfirmed = cell_value;
                        break;
                    case 9:
                        lineData.ReasonType = cell_value;
                        break;
                    case 10:
                        lineData.OtherPartyInfo = cell_value;
                        break;
                    case 11:
                        lineData.LinkedTransactionID = cell_value;
                        break;
                    case 12:
                        lineData.AccNo = cell_value;
                        break;
                }

                if (C === IDX_LAST_LINE_COL) {
                    statementLines.push(lineData);
                }
            }
        }

        // set server variable
        statementFile.StatementFileName = selectedStatementFile.name;
        parsedStatementFile.METADATA = statementFile;
        parsedStatementFile.Transactions = statementLines;

    });
}

function ExtractFileMetaData(workSheet) {
    var statementFile = {
        ShortCode: null,
        Operator: null,
        Organization: null,
        ReportDate: null,
        StatementFileName: null
    };
    var IDX_LAST_METADATA_ROW = 4;

    var cell_ref;
    var cell_value;

    for (var R = 0; R <= IDX_LAST_METADATA_ROW; ++R) {
        switch (R) {
            case 1: // extract shortcode
                cell_ref = XLSX.utils.encode_cell({ c: 1, r: R });
                cell_value = ExtractCellData(workSheet[cell_ref]);

                statementFile.ShortCode = cell_value;
                break;
            case 4:
                for (var C = 1; C <= 5; C += 2) {
                    cell_ref = XLSX.utils.encode_cell({ c: C, r: R });
                    cell_value = ExtractCellData(workSheet[cell_ref]);

                    switch (C) {
                        case 1:
                            statementFile.Operator = cell_value;
                            break;
                        case 3:
                            statementFile.Organization = cell_value;
                            break;
                        case 5:
                            statementFile.ReportDate = moment(cell_value).format(PORTAL_DATE_FORMAT);
                            break;
                    }
                }
                break;
        }
    }

    return statementFile;
}

function IsLineHeaderRowPresent(workSheet) {
    var headerRowsPresent = false;

    var IDX_LINE_HEADER_ROW = 5;
    var IDX_LAST_HEADER_COL = 12;

    for (var C = 0; C <= IDX_LAST_HEADER_COL; ++C) {

        var header = workSheet[XLSX.utils.encode_cell({ c: C, r: IDX_LINE_HEADER_ROW })];
        switch (C) {
            case 0:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Receipt No");
                break;
            case 1:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Completion Time");
                break;
            case 2:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Initiation Time");
                break;
            case 3:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Details");
                break;
            case 4:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Transaction Status");
                break;
            case 5:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Paid In");
                break;
            case 6:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Withdrawn");
                break;
            case 7:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Balance");
                break;
            case 8:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Balance Confirmed");
                break;
            case 9:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Reason Type");
                break;
            case 10:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Other Party Info");
                break;
            case 11:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "Linked Transaction ID");
                break;
            case 12:
                headerRowsPresent = header !== undefined && DoesStringContainSubString(header.v, "A/C No");
                break;
        }

        if (!headerRowsPresent) return;
    }

    return headerRowsPresent;
}

function ExtractCellData(sheetjs_cell) {
    return sheetjs_cell ? sheetjs_cell.v : null;
}

function UploadStatement(restUrl, statement, apiCommParams) {
    var ajaxParams = {
        statementFile: statement.METADATA,
        lines: statement.Transactions,
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
        ParseDepositUploadResponse(response);
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

        ParseDepositUploadResponse(msg);
    };


    CallMSACCO(restUrl, ajaxParams, apiCommParams, msaccoCallBack);
}

function ParseDepositUploadResponse(serverResponse) {
    var msg;
    if (serverResponse.success == true) {
        msg = "<h4>Status: Success</h4>  <p/><p/>" + serverResponse.ex + "<p/><p/>Statement file successfully uploaded"
    } else {
        msg = "<h4>Status: Failed</h4> <p/><p/>" + serverResponse.ex;
    }

    bootbox.alert({
        title: "<h4>MSACCO Deposits</h4>",
        message: msg
    });
}