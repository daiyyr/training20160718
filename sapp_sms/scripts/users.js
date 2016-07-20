function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

function ImageButtonEdit_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridUsers") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ImageButtonEdit|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}