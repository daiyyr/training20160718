
function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}
function ImageButtonUp_ClientClick() {
    var grid = jQuery("#" + GetClientId("JqGridUnpaidTrans") + "_datagrid1");
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
    var grid = jQuery("#" + GetClientId("jqGridTransRelated") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");
    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ImageButtonDown|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }
    return false;
}



function IsDecimal(value) {
    var regex = /^(\+|-)?([0-9]*\.?[0-9]*)$/;
    if (regex.test(value))
        return true;
    else
        return false;
}

function ValidateRowData(value, colname) {
    if (value == "") {
        return [false, colname+' Required'];
    }
    else {
        if (colname == "Paid") {
            if (!IsDecimal(value)) {
                return [false, colname + 'should be decimal'];
            }
        }
        else {
            return [true, ''];
        }
        return [true, ''];
    }
}

function ChangeGross() {
    var grid = jQuery("#" + GetClientId("jqGridTrans") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");
    rowKeys = rowKeys + '';
    rowKeys = rowKeys.trim();
    if (rowKeys != "") {
        var grossSum = 0;
        var keyss = rowKeys.split(',');
        $.each(keyss, function (n, value) {
            var gross = grid.getCell(value, 'Gross');
            grossSum += parseFloat(gross);
        });
        var grosslabel = jQuery("#" + GetClientId("LabelGross"));
        grosslabel.attr("innerHTML", grossSum);
    }
    else {
        var grosslabel = jQuery("#" + GetClientId("LabelGross"));
        grosslabel.attr("innerHTML", "0");
    }
}

function afterRowSave() {
    $.ajax({
        type: "POST",
        url: "cpaymentedit.aspx/GetGross",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#LabelGross").html(msg.d);
        }
    });
}

function selectrows() {
    var grid = jQuery("#" + GetClientId("jqGridTrans") + "_datagrid1");
    grid.setSelection(2, false);
}