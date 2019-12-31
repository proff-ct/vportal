var tblTabulator;
var tabulatorAjaxUrlForReload;
var tabulatorAjaxParamsForReload;

function initTabulator(tableContainerID, editUrl) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Records of Configured Float Alerts Found ",
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
      {
        formatter: editRecord,
        formatterParams: { editUrl: editUrl },
        align: "center",
        headerSort: false
      },
      { title: "SACCO", field: "SaccoName", headerFilter: true },
      {
        title: "Float Resource", field: "FloatResource",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Alert Level", field: "AlertType",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },{
        title: "Alert Threshold", field: "Threshold",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },{
        title: "Alert Trigger Condition", field: "TriggerCondition",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Set By", field: "CreatedBy",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Date Set", field: "CreatedOn",
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
      }
      //{ title: "Corporate No", field: "ClientCorporateNo" },
    ],
    movableColumns: true,
    index: "Id",
    groupBy: "FloatResource",
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
  //var utcDate = moment.utc("2015-01-30 10:00:00");
  //var localDate = moment(utcDate).local();
  return (objDate == null) ?
    'No Date' : moment.utc(objDate).local().format("DD-MMM-YYYY hh:mm:ss A")
}

function ClearFilters() {
  tblTabulator.clearFilter(true);
}

function LoadData(restUrl, ajaxParams=null) {
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
    "?clientFloatAlertId=" +
    cell.getRow().getData().Id +
    "'><i class='fa fa-edit' style='color: red'></i></a>";
};

// Below are used in the AddOrUpdate view

function LoadConfigurableAlerts(restUrl, ajaxParams, idOfSelectToLoad) {
  // make an ajax call to refresh the passed in select box
  $.ajax({
    type: "GET",
    url: restUrl,
    data: {
      clientCorporateNo: ajaxParams.clientCorporateNo,
      floatResourceId: ajaxParams.floatResourceId
    },
    success: function (data) {
      //call is successfully completed and we got result in data
      if (data == '') return;
      LoadSelectElement(idOfSelectToLoad, data);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert("Server Error Occurred. Unable to retrieve the configurable alert levels");
      //Log(xhr.responseText);
    }
  });
}

function LoadSelectElement(selectRef, data) {
  $(selectRef).children('option:not(:first)').remove();
  $.each(data, function (index, clientListItem) {
    var c = new Option(clientListItem.Text, clientListItem.Value);
    $(selectRef).append(c);
  });
  $(selectRef).selectpicker('refresh');
  //alert("Client list (re)loaded");
}

function ResetSelectPickerElement(selectRef) {
  $(selectRef).children('option:not(:first)').remove();
  $(selectRef).selectpicker('refresh');
}