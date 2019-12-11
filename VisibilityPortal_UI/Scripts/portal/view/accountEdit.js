var tblTabulator;

function initTabulator(tableContainerID, portalUserRolesArray) {
  //create Tabulator on DOM element with id == tableContainerID
  tblTabulator = new Tabulator(tableContainerID, {
    //height: 405, // set height of table (in CSS or here), this enables the Virtual DOM and improves render speed dramatically (can be any valid css height value)
    ////data: tabledata, //assign data to table
    placeholder: "No Portal Role Data Found ",
    // collapse columns that no longer fit on the table into a list under the row
    responsiveLayout: "collapse",
    responsiveLayoutCollapseStartOpen: false,
    layout: "fitDataFill", //fit columns to width of table (optional),
    headerFilterPlaceholder: "filter ...",
    tooltipsHeader: true,
    data: portalUserRolesArray,
    columns: [ //Define Table Columns
      {
        formatter: "responsiveCollapse", width: 30, minWidth: 30,
        align: "center", resizable: false, headerSort: false
      },
      {
        title: "Module", field: "Module",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Role", field: "AspRoleName",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Assigned By", field: "CreatedBy",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true }
      },
      {
        title: "Date Assigned", field: "CreatedOn",
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
        title: "User Role Enabled?", field: "IsEnabled",
        headerFilter: "select", headerFilterFunc: "=",
        headerFilterParams: { values: true },
        editor: true,
        formatter: "tickCross"
      },
    ],
    movableColumns: true,
    index: "ClientModuleId",
    initialSort: [
      { column: "Module", dir: "asc" }
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

//custom formatter definition
var editRecord = function (cell, formatterParams, onRendered) { //plain text value

  return "<a href='" +
    formatterParams.editUrl +
    "?clientModuleId=" +
    cell.getRow().getData().ClientModuleId +
    "'><i class='fa fa-edit' style='color: red'></i></a>";
};

function SetPlaceholderText(placeholderId, placeholderText) {
  $(placeholderId).text(placeholderText);
}

function ProcessSubmit(e, oForm, portalRolesFormName) {
  // this gets the module user role data and adds it to the form data being submitted
  //e.preventDefault(); // commented out because this function isn't being used in a 'submit' handler
  var userRoles = tblTabulator.getData();

  $.each(userRoles, function (index, value) {
    oForm.elements[portalRolesFormName + '[' + index + '].UserId'].value = value.UserId;
    oForm.elements[portalRolesFormName + '[' + index + '].ClientModuleId'].value = value.ClientModuleId;
    oForm.elements[portalRolesFormName + '[' + index + '].AspRoleId'].value = value.AspRoleId;
    oForm.elements[portalRolesFormName + '[' + index + '].IsEnabled'].value = value.IsEnabled;
  });

  oForm.submit();
}