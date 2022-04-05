// here, the index maps to the error code returned from getValidationError - see readme
var itiErrorMap = ["Invalid number", "Invalid country code", "Too short", "Too long", "Invalid number"]
var PORTAL_DATE_FORMAT = "DD-MMM-YYYY hh:mm:ss A";

function IsITINumberValid(elementID, alertTitle, callback = null) {
  elementID = "#" + elementID;
  if (!document.querySelector(elementID)) throw "HTML ref for phone number is invalid";

  if ($(elementID).val() == '') {
    bootbox.alert({
      title: "<h4>" + alertTitle + " - Phone Number Error</h4>",
      message: "Phone number not supplied"
    });
    return false;
  }
  else if (!$(elementID).intlTelInput("isValidNumber")) {
    bootbox.alert({
      title: "<h4>" + alertTitle + " - Phone Number Error</h4>",
      message: itiErrorMap[$(elementID).intlTelInput("getValidationError")]
    });
    return false;
  }
  else {
    if (callback) callback($(elementID).intlTelInput("getNumber"));
    return true;
  }

}

function initSelectPicker(selectRef) {
  $(selectRef).selectpicker({
    liveSearch: true,
    showTick: true
  });
}

function tabulatorClearFilters() {
  this.clearFilter(true);
}

function TabulatorSetCellValueToNullIfNoData(cellValue, valueToSet = null) {
  return (cellValue == null) ? (valueToSet != null) ? valueToSet : "No Data" : cellValue
}

function DoesStringContainSubString(stringToCheck, subStringToLookFor) {
  return stringToCheck.indexOf(subStringToLookFor) !== -1;
}

function PopupMessage(alertTitle, alertMessage, titleOverride = null) {
    var displayTitle;

    if (titleOverride == null || titleOverride == undefined) {
        displayTitle = "<h5>" + alertTitle + "</h5>"
    }
    else {
        displayTitle = titleOverride;
    }

    bootbox.alert({
        title: displayTitle,
        message: alertMessage
    });
}

function ToastPrompt(promptMessage) {

}