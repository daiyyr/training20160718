function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

function confirm_delete() {
    if (confirm("Are you sure you want to delete the item?") == true)
        return true;
    else
        return false;
}