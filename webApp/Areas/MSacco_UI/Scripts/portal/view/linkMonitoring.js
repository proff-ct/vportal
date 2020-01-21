var tblTabulatorLinkMonitoring;
var tabulatorLinkMonitoringAjaxUrlForReload;
var tabulatorLinkMonitoringAjaxParamsForReload;

function initTabulator(tableContainerID) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulatorLinkMonitoring = new Tabulator(tableContainerID, {
    height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    //data: tabledata, //assign data to table
    placeholder: "No Link Status Records Found ",
    pagination: "remote", // setting this to local v4.4 has buggy behaviour in which not all columns show
    paginationSize: 5,
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
      //{ title: "Corporate No", field: "Corporate_No", visible: "false" },
      { title: "Entity", field: "Corporate_Name", headerFilter: false, headerSort: false },
      {
        title: "Coretec-to-Sacco Link Status", field: "Http_Status", formatter: "textarea",
        headerFilter: false, headerSort: false
      },
      {
        title: "Time Last Checked", field: "Last_Check",
        align: "center", headerFilter: false, headerSort: false,
        formatter: function (cell, formatterParams) {
          return GetFormattedDate(cell.getValue());
        }
      },
      //{
      //  title: "Time Last Email Sent", field: "Last_Email_Sent",
      //  align: "center", headerFilter: true,
      //  formatter: function (cell, formatterParams) {
      //    return GetFormattedDate(cell.getValue());
      //  }
      //},
    ],
    movableColumns: true,
    index: "Corporate_No",
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

      initDowntimeSubTable(tableEl, row.getData().Downtimes);
    },
    renderComplete: function () {
      // make guarantor subtables invisible by default
      HideDowntimeSubTables(this);
    },
    pageLoaded: function (pageno) {
      //pageno - the number of the loaded page
      //alert(pageno)
      SortLinkMonitoringData(this);
      HideDowntimeSubTables(this);
    }

  });
  $(tblTabulatorLinkMonitoring.element).addClass("table table-striped table-condensed table-hover");

}
function initDowntimeSubTable(tableElement, tableData) {
  var subTable = new Tabulator(tableElement, {
    layout: "fitDataFill",
    data: tableData,
    placeholder: "No Downtime Data ",
    responsiveLayout: "collapse",
    columns: [{
      formatter: "responsiveCollapse", width: 30, minWidth: 30,
      align: "center", resizable: false, headerSort: false
    },
    {
      title: "Entry No",
      field: "Entry_No"
    },
    {
      title: "Start",
      field: "Downtime_Start",
      align: "center",
      headerTooltip: "Down time start",
      formatter: function (cell, formatterParams) {
        return GetFormattedDate(cell.getValue());
      }
    },
    {
      title: "End",
      field: "Downtime_Stop",
      align: "center",
      headerTooltip: "Down time end",
      formatter: function (cell, formatterParams) {
        return GetFormattedDate(cell.getValue());
      }
    },
    {
      title: "Downtime Description",
      field: "Comment"
    },
    {
      title: "Url",
      field: "Url"
    },
    {
      title: "Last Checked",
      field: "Last_Check",
      align: "center",
      formatter: function (cell, formatterParams) {
        return GetFormattedDate(cell.getValue());
      }
    }
    ]
  })
}

function HideDowntimeSubTables(tabulatorTableRef) {
  // how do I toggle visibility?
  // - get the visibile rows
  // - get the row id's
  // - for each id, hide the subtable
  // when do I toggle visibility?
  // - when the page finishes rendering
  var pageRows = tabulatorTableRef.getRows();
  pageRows.forEach(function (pageRow) {
    // get reference to child table element and hide
    $(pageRow.getElement()).children("div.subTable" + pageRow.getIndex()).toggle(false);
  });
}


function GetFormattedDate(objDate) {
  return (objDate == null) ?
    'No Date' : moment(objDate).format("DD-MMM-YYYY hh:mm:ss A")
}

function ClearFilters() {
  tblTabulatorLinkMonitoring.clearFilter(true);
}

function LoadData(restUrl, corporateNo, getAll = false) {
  if (!getAll && (corporateNo == '' || corporateNo == undefined)) return;
  var ajaxParams = {
    clientCorporateNo: corporateNo,
    loadAll: getAll
  };

  tabulatorLinkMonitoringAjaxUrlForReload = restUrl;
  tabulatorLinkMonitoringAjaxParamsForReload = ajaxParams

  ClearFilters();
  tblTabulatorLinkMonitoring.setData(restUrl, ajaxParams);
}

function ReloadData() {
  tblTabulatorLinkMonitoring.setData(tabulatorLinkMonitoringAjaxUrlForReload, tabulatorLinkMonitoringAjaxParamsForReload);
}


function SortLinkMonitoringData(tabulatorTableRef) {
  tabulatorTableRef.setSort([
    { column: "Corporate_Name", dir: "asc" }, //sort by this first
    { column: "Http_Status", dir: "desc" }, //then sort by this second
  ]);
}

/**
 * MatAdo 02-Jan-2020: Code note:
 * Added some different styles for invoking functions e.g.
 * 1. SortLinkMonitoringData(tabulatorTableRef) 
 *      -> called and passed 'this'
 * 2. ReloadData = ReloadDataTabulator.bind(tblTabulatorLinkMonitoring);
 *      -> is bound to the tblTabulatorLinkMonitoring object so I don't have to use the variable name
 * These are in readiness for sprucing up the js code now that we're nearing full on deployment to
 * production. 
 * The point of them therefore is to help refresh the memory when next I revisit this code
 * 
 **/ 