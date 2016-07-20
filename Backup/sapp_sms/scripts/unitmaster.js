function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

function querySt(ji) {
    hu = window.location.search.substring(1);
    gy = hu.split("&");
    for (i = 0; i < gy.length; i++) {
        ft = gy[i].split("=");
        if (ft[0] == ji) {
            return ft[1];
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

function ImageButtonReports_ClientClick() {
    var propertyid = querySt("propertyid");
    if (propertyid != null) {
        if (propertyid != "") {
            window.open('unitreports.aspx?propertyid=' + propertyid, '_blank');
        }
        else
            window.open('unitreports.aspx', '_blank');
    }
    else
        window.open('unitreports.aspx', '_blank');
}
