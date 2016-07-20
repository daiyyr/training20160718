
$(document).ready(function () {
    window.onunload = function () {
        $.cookie('Code', $('#TextBoxCode').val());
        $.cookie('Area', $('#TextBoxArea').val());
        $.cookie('OwnerShipInterest', $('#TextBoxOwnershipInterest').val());
        $.cookie('UtilityInterest', $('#TextBoxUtilityInterest').val());
        $.cookie('SpecialScale', $('#TextBoxSpecialScale').val());
        $.cookie('Notes', $('#TextBoxNotes').val());
        $.cookie('SetTime', $.now());
    }
    window.onload = function () {
        var lastTime = $.cookie('SetTime');
        var curTime = $.now();
        if (curTime - lastTime < 5000) {
            if ($.cookie('Code') != null)
                $('#TextBoxCode').val($.cookie('Code'));
            if ($.cookie('Area') != null)
                $('#TextBoxArea').val($.cookie('Area'));
            if ($.cookie('OwnerShipInterest') != null)
                $('#TextBoxOwnershipInterest').val($.cookie('OwnerShipInterest'));
            if ($.cookie('UtilityInterest') != null)
                $('#TextBoxUtilityInterest').val($.cookie('UtilityInterest'));
            if ($.cookie('SpecialScale') != null)
                $('#TextBoxSpecialScale').val($.cookie('SpecialScale'));
            if ($.cookie('Notes') != null)
                $('#TextBoxNotes').val($.cookie('Notes'));
        }
    }
});


function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

