function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

function ImageButtonUp_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridUnreconciled") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");
    if (rowKeys) {
        var ids = "";
        rowKeys = rowKeys + '';
        var keyss = rowKeys.split(',');
        $.each(keyss, function (n, value) {
            var rowID = grid.getCell(value, 'ID');
            ids = ids + rowID + ",";
        });
        ids = ids.substring(0, ids.length - 1);
        __doPostBack('__Page', 'ImageButtonUp|' + ids);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }
    return false;
}

function ImageButtonDown_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridReonciled") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");
    if (rowKeys) {
        var ids = "";
        rowKeys = rowKeys + '';
        var keyss = rowKeys.split(',');
        $.each(keyss, function (n, value) {
            var rowID = grid.getCell(value, 'ID');
            ids = ids + rowID + ",";
        });
        ids = ids.substring(0, ids.length - 1);
        __doPostBack('__Page', 'ImageButtonDown|' + ids);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }
    return false;
}

function getQueryString(name) {
    if (location.href.indexOf("?") == -1 || location.href.indexOf(name + '=') == -1) {
        return '';
    }
    var queryString = location.href.substring(location.href.indexOf("?") + 1);
    var parameters = queryString.split("&");
    var pos, paraName, paraValue;
    for (var i = 0; i < parameters.length; i++) {
        pos = parameters[i].indexOf('=');
        if (pos == -1) { continue; }
        paraName = parameters[i].substring(0, pos);
        paraValue = parameters[i].substring(pos + 1);
        if (paraName == name) {
            return unescape(paraValue.replace(/\+/g, " "));
        }
    }
    return '';
};


function ImageButtonInsert_ClientClick() {
    var url = "bankreconcileinsert.aspx?accountid=" + getQueryString("accountid");
    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 1000px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
    if (vReturnValue == "refresh") {
        __doPostBack('__Page', 'Refresh|0');
    }

}