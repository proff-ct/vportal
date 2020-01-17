function initSelectPicker(selectRef) {
  $(selectRef).selectpicker({
    liveSearch: true,
    showTick: true
  });
}

function tabulatorClearFilters() {
  this.clearFilter(true);
}