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
