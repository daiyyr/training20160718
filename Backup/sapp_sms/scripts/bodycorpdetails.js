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

function ButtonOKClick() {
    var bcid = $("#HiddenBCID").val();
    var startdate = $("#TextBoxStartDate").val();
    window.location = "budgetmaster.aspx?bodycorpid="+bcid+"&startdate=" + startdate;
}

function ButtonActivityOKClick() {
    var bcid = $("#HiddenBCID").val();
    var startdate = $("#TextBoxActivityStart").val();
    var enddate = $("#TextBoxActivityEnd").val();
    window.location = "activity.aspx?mode=bodycorp&id=" + bcid + "&start=" + startdate + "&end=" + enddate;
}

function ButtonDownload_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridFiles") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'FileName');
        var Type = grid.getCell(rowKey, 'Type');
        if (Type != 'DIR') {
            __doPostBack('__Page', 'ButtonDownload|' + ID);
        } else {
            alert("Please select a file!");
        }
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ButtonDelete_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridFiles") + "_datagrid1");
        var rowKey = grid.getGridParam("selrow");

        if (rowKey) {
            var ID = grid.getCell(rowKey, 'FileName');
            var Type = grid.getCell(rowKey, 'Type');
            __doPostBack('__Page', 'ButtonDelete|' + ID + '|' + Type);
            return false;
        }
        else {
            alert("Please select a row first!");
            return false;
        }
    }
    return false;
}

function ImageButtonReports_ClientClick() {
    var bodycorp_id = querySt("bodycorpid");
    window.open('financialreports.aspx?bodycorpid=' + bodycorp_id, '_blank');
}

function ImageButtonLevies_ClientClick() {
    var bodycorp_id = querySt("bodycorpid");
    window.open('levylist.aspx?bodycorpid=' + bodycorp_id, '_blank');
}

function ButtonEnter_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridFiles") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'FileName');
        var Type = grid.getCell(rowKey, 'Type');
        if (Type = 'DIR') {
            __doPostBack('__Page', 'ButtonEnter|' + ID);
            return false;
        }
        else {
            alert("Please select a dir!");
            return false;
        }
    }
    else {
        alert("Please select a dir!");
        return false;
    }
    return false;
}