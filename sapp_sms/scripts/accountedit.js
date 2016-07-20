function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

window.onunload = refreshParent;
function refreshParent() {
    if (null != window.opener) {
        window.opener.location.reload();
    }
}