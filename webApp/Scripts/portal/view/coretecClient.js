var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, editUrl) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Records of Registered Clients Found ",
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
      { title: "SACCO", field: "SaccoName", headerFilter: true },
      {
        title: "Portal Module", field: "PortalModuleName",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Registered By", field: "CreatedBy",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Date Registered", field: "CreatedOn",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      {
        title: "Date Last Modified", field: "ModifiedOn",
        align: "center", headerFilter: true,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      {
        title: "Is Module Enabled?", field: "IsEnabled",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      //{ title: "Corporate No", field: "ClientCorporateNo" },
      {
        formatter: editRecord,
        formatterParams: { editUrl: editUrl },
        align: "center",
        headerSort: false
      }
    ],
    movableColumns: true,
    index: "ClientModuleId",
    groupBy: "SaccoName",
    groupToggleElement: "header", //toggle group on click anywhere in the group header
    groupStartOpen: false,
    initialSort: [
      { column: "SaccoName", dir: "asc" }
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

function LoadData(restUrl) {
  var ajaxParams = null;

  tabulatorAjaxUrlForReload = restUrl;
  tabulatorAjaxParamsForReload = ajaxParams

  ClearFilters();
  tblTabulator.setData(restUrl, ajaxParams);
}

function ReloadData() {
  tblTabulator.setData(tabulatorAjaxUrlForReload, tabulatorAjaxParamsForReload);
}

//custom formatter definition
var editRecord = function (cell, formatterParams, onRendered) { //plain text value

  return "<a href='" +
    formatterParams.editUrl +
    "?clientModuleId=" +
    cell.getRow().getData().ClientModuleId +
    "'><i class='fa fa-edit' style='color: red'></i></a>";
};

// Below are used in the AddOrUpdate view


function InitMultiselect(multiselectId, multiselectOptions = null) {
  $(multiselectId).multiSelect(multiselectOptions);
}
function LoadClientList(restUrl, selectId) {
  // make an ajax call to refresh the passed in select box
  $.ajax({
    type: "GET",
    url: restUrl,
    data: { mode: "forSelect" },
    success: function (data) {
      //call is successfully completed and we got result in data
      if (data == '') return;
      LoadClientSelect(selectId, data);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert("Server Error Occurred. Unable to load list of unregistered clients");
      //Log(xhr.responseText);
    }
  });
}

function LoadClientSelect(selectRef, data) {
  $(selectRef).children('option:not(:first)').remove();
  $.each(data, function (index, clientListItem) {
    var c = new Option(clientListItem.Text, clientListItem.Value);
    $(selectRef).append(c);
  });
  $(selectRef).selectpicker('refresh');
  //alert("Client list (re)loaded");
}

function InitSelectPicker(selectRef) {
  $(selectRef).selectpicker({
    liveSearch: true,
    showTick: true
  });
}

function SetPlaceholderText(placeholderId, placeholderText) {
  $(placeholderId).text(placeholderText);
}