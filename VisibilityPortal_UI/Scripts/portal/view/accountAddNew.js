var clientsWithModules = [];

function LoadClientList(restUrl, selectId) {
  // make an ajax call to refresh the passed in select box
  $.ajax({
    type: "GET",
    url: restUrl,
    data: {
      page: 1,
      size: 1,
      mode: "forSelect"
    },
    success: function (data) {
      //call is successfully completed and we got result in data
      if (data == '') return;
      clientsWithModules = data;
      LoadClientSelect(selectId, data);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert("Server Error Occurred. Unable to load list of registered clients");
      //Log(xhr.responseText);
    }
  });
}

function LoadClientSelect(selectRef, data) {
  $(selectRef).children('option:not(:first)').remove();
  $.each(data, function (index, clientWithModule) {
    var c = new Option(clientWithModule.saccoName_1, clientWithModule.corporateNo);
    $(selectRef).append(c);
  });
  $(selectRef).selectpicker('refresh');
}

function SetPlaceholderText(placeholderId, placeholderText) {
  $(placeholderId).text(placeholderText);
}

function SetClientModules(clientCorporateNo, clientModuleDropdownId) {
    $(clientModuleDropdownId).children('option:not(:first)').remove();
    $.each(clientsWithModules, function (key, objClientWithModules) {
      if (objClientWithModules.corporateNo == clientCorporateNo) {
        $.each(objClientWithModules.Modules, function (index, clientModule) {
          $(clientModuleDropdownId).append(
            $("<option/>").val(clientModule.ClientModuleId).html(clientModule.PortalModuleName));
          $(clientModuleDropdownId).selectpicker('refresh');
        });
        // exit the loop
        return false;
      }
    });
}
function ResetModuleAndRole(selectModuleRef, selectRoleRef) {
  $(selectModuleRef).children('option:not(:first)').remove();
  $(selectModuleRef).selectpicker('refresh');
  $(selectRoleRef).val('').change();
}