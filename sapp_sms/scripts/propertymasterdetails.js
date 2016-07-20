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
    if (colname == "Name(*)") {
        if (value == "") {
            return [false, 'Name Required'];
        }
        else {
            return [true, ''];
        }
    }
    else {
        return [true, ''];
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

function ButtonDeleteContact_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridContacts") + "_datagrid1");
    var test = GetClientId("jqGridContacts");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ButtonDeleteContact|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!c");
        return false;
    }

    return false;
}


function ButtonDeleteContactComm_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var subgrid = jQuery("#" + GetClientId("jqGridContacts") + "_list10_d");
        var mastergrid = jQuery("#" + GetClientId("jqGridContacts") + "_datagrid1");
        var rowKeySub = subgrid.getGridParam("selrow");
        var rowKeyMaster = mastergrid.getGridParam("selrow");
        if (rowKeySub && rowKeyMaster) {
            var subID = subgrid.getCell(rowKeySub, 'ID');
            var masterID = mastergrid.getCell(rowKeyMaster, 'ID');
            __doPostBack('__Page', 'ButtonDeleteContactComm|' + subID + "|" + masterID);
            return false;
        }
        else {
            alert("Please select a row first!cc");
            return false;
        }
    }
    else
        return false;
}

