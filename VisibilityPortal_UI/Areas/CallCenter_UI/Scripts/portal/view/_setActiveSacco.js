function LoadSaccoList(restUrl, selectId) {
  // make an ajax call to refresh the passed in select box
  $.ajax({
    type: "GET",
    url: restUrl,
    data: { mode: "forSelect" },
    success: function (data) {
      //call is successfully completed and we got result in data
      if (data == '') return;
      LoadSaccoSelect(selectId, data);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert("Server Error Occurred. Unable to load list of saccos");
      //Log(xhr.responseText);
    }
  });
}

function LoadSaccoSelect(selectRef, data) {
  $(selectRef).children('option:not(:first)').remove();
  $.each(data, function (index, saccoListItem) {
    var s = new Option(saccoListItem.Text, saccoListItem.Value);
    $(selectRef).append(s);
  });
  $(selectRef).selectpicker('refresh');
  //alert("Sacco list (re)loaded");
}

function SetActiveSacco(saccoCorporateNo, placeholderControl) {
  $.ajax({
    type: "POST",
    url: "Sacco/SetActiveSacco",
    data: { corporateNo: saccoCorporateNo },
    success: function (data) {
      //call is successfully completed and we got result in data
      SetPlaceholderText(
        placeholderControl.placeholderContainerId, placeholderControl.placeholder, data);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert("Server Error Occurred. Unable to Set Active Sacco");
      //Log(xhr.responseText);
    }
  });
}

function SetPlaceholderText(placeholderContainer, placeholderId, placeholderText) {
  $(placeholderContainer).find(placeholderId).text(placeholderText);
}
