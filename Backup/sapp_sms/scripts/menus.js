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

function ButtonSubEdit_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridMenus") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ButtonSubEdit|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ButtonSubDelete_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridMenus") + "_datagrid1");
        var rowKey = grid.getGridParam("selrow");

        if (rowKey) {
            var ID = grid.getCell(rowKey, 'ID');
            __doPostBack('__Page', 'ButtonSubDelete|' + ID);
            return false;
        }
        else {
            alert("Please select a row first!");
            return false;
        }

        return false;
    }
    else
        return false;
}

function ButtonMoveUp_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridMenus") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ButtonMoveUp|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ButtonMoveDown_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridMenus") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ButtonMoveDown|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}