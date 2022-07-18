function initSelectPicker(selectRef) {
  $(selectRef).selectpicker({
    liveSearch: true,
    showTick: true
  });
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