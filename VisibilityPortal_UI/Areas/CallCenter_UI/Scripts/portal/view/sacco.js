function initTable(tableId) {
  $(tableId).DataTable({
    'paging': true,
    'lengthChange': true,
    'searching': true,
    'ordering': true,
    'info': true,
    'autoWidth': false,
    'columnDefs': [{
      'targets': [3], // column index (start from 0)
      'orderable': false, // set orderable false for selected columns
      'searchable': false, // set searchable false for selected columns
    }]
  });
}
function SetActiveSacco(saccoCorporateNo, placeholderControl, activeSacco) {
  $.ajax({
    type: "POST",
    url: "Sacco/SetActiveSacco",
    data: { corporateNo: saccoCorporateNo },
    success: function (data) {
      //call is successfully completed and we got result in data
      SetPlaceholderText(placeholderControl.controlID, placeholderControl.placeholder, data);
      if (!activeSacco) location.reload();
      alert("Set Active Sacco Success");
      // redirect to the InstantLoan Products list view
      //window.location.href = '/ProductFactory/ListInstantLoanProducts';
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert("Server Error Occurred. Unable to Set Active Sacco");
      //Log(xhr.responseText);
    }
  });
}

function SetPlaceholderText(controlID, controlPlaceholder, placeholderText) {
  $(controlID).find(controlPlaceholder).text(placeholderText);
}
