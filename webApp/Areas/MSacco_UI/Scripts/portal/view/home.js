function DisplayDateNow(dateFormat = null) {
  return moment().format((dateFormat == null) ? "DD-MMM-YYYY HH:mm:ss" : dateFormat )
}

function DisplayDefaultQuantity(defaultQty = null) {
  return defaultQty || "---"
}