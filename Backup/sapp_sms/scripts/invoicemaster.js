﻿function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

function ImageButtonEdit_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
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


function ImageButtonCopyAdd_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var copiedMasterId = grid.getCell(rowKey, 'ID');

        __doPostBack('__Page', 'ImageButtonCopyAdd|' + copiedMasterId);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}


function ImageButtonDelete_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
        var rowKey = grid.getGridParam("selrow");

        if (rowKey) {
            var ID = grid.getCell(rowKey, 'ID');
            __doPostBack('__Page', 'ImageButtonDelete|' + ID);
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

function ImageButtonDetails_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ImageButtonDetails|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ImageButtonAllocate_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ImageButtonAllocate|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}