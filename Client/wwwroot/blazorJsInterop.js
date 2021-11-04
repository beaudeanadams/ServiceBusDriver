
// Get User's Timezone
function GetTimezoneValue() {
    return new Date().getTimezoneOffset();
}


// Scroll trace logs to bottom automatically
function ScrollToBottom(id) {
    var element = document.getElementById(id);
    element.scrollTop = element.scrollHeight;
}