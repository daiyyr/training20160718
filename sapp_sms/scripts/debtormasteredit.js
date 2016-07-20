$(document).ready(function () {
    $("#TextBoxName").blur(function () {
        var Name = jQuery("#TextBoxName");
        var Sal = jQuery("#TextBoxSalutation");
        Sal.val(Name.val());
    });
});



window.onunload = refreshParent;
function refreshParent() {
    if (null != window.opener) {
        window.opener.location.reload();
    }
}