$(document).ready(function () {
    $('#TextBoxAgmTime').timepicker();
    $('#TextBoxCommitteeTime').timepicker();
    $('#TextBoxEgmTime').timepicker();
    
});


function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

