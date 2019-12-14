function LoadRolesList(restUrl, selectId, ajaxParams) {
  // make an ajax call to refresh the passed in select box
  $.ajax({
    type: "GET",
    url: restUrl,
    data: {
      email: ajaxParams.Email,
      clientModuleId: ajaxParams.ClientModuleId
    },
    success: function (data) {
      //call is successfully completed and we got result in data
      if (data == '') return;
      LoadRolesSelect(selectId, data);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      //some errror, some show err msg to user and log the error
      alert("Server Error Occurred. Unable to load list of available roles");
      //Log(xhr.responseText);
    }
  });
}

function LoadRolesSelect(selectRef, availableRoles) {
  $(selectRef).children('option:not(:first)').remove();
  $.each(availableRoles, function (index, role) {
    var r = new Option(role.Name, role.RoleId);
    $(selectRef).append(r);
  });
  $(selectRef).selectpicker('refresh');
}

function ResetRoleSelect(selectRoleRef) {
  $(selectRoleRef).children('option:not(:first)').remove();
  $(selectRoleRef).selectpicker('refresh');
}