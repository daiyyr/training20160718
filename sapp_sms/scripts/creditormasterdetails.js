function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}
$(function () {
    $("#tabs").tabs();
});

function ValidateRowData(value, colname) {
    if (colname == "Details(*)") {
        if (value == "") {
            return [false, 'Details Required'];
        }
        else {
            return [true, ''];
        }
    }
    else {
    }
}

function ButtonDeleteComm_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridComms") + "_datagrid1");
    var test = GetClientId("jqGridComms");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ButtonDeleteComm|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}