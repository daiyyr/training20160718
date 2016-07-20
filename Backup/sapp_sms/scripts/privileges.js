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

function confirm_delete() {
    if (confirm("Are you sure you want to delete the item?") == true)
        return true;
    else
        return false;
}

function ImageButtonMoveLeft_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridIncludedForms") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");

    if (rowKeys.length > 0) {
        var IDs = "{\"IDs\":[";
        for (i = 0; i < rowKeys.length; i++) {
            if (i != (rowKeys.length - 1)) IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\", ";
            else IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\"";
        }
        IDs += "]}";
        __doPostBack('__Page', 'ImageButtonMoveLeft|' + IDs);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ImageButtonMoveRight_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridForms") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");

    if (rowKeys.length > 0) {
        var IDs = "{\"IDs\":[";
        for (i = 0; i < rowKeys.length; i++) {
            if(i != (rowKeys.length -1)) IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\", ";
            else IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\"";
        }
        IDs += "]}";
        __doPostBack('__Page', 'ImageButtonMoveRight|' + IDs);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ImageButtonMenuLeft_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridIncludedMenus") + "_datagrid1");

    var rowKeys = grid.getGridParam("selarrrow");

    if (rowKeys.length > 0) {
        var IDs = "{\"IDs\":[";
        for (i = 0; i < rowKeys.length; i++) {
            if (i != (rowKeys.length - 1)) IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\", ";
            else IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\"";
        }
        IDs += "]}";
        __doPostBack('__Page', 'ImageButtonMenuLeft|' + IDs);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ImageButtonMenuRight_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridMenus") + "_datagrid1");

    var rowKeys = grid.getGridParam("selarrrow");

    if (rowKeys.length > 0) {
        var IDs = "{\"IDs\":[";
        for (i = 0; i < rowKeys.length; i++) {
            if (i != (rowKeys.length - 1)) IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\", ";
            else IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\"";
        }
        IDs += "]}";
        __doPostBack('__Page', 'ImageButtonMenuRight|' + IDs);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}