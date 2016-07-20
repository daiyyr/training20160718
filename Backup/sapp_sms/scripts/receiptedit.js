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
        return [false, colname + ' Required'];
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


function ChangeNet(oldvalue, value) {
    var net = jQuery("#" + GetClientId("TextBoxNet"));
    var tax = jQuery("#" + GetClientId("TextBoxTax"));
    var gross = jQuery("#" + GetClientId("TextBoxGross"));
    if (net.val() == "") net.val(0);
    if (oldvalue == "" || IsDecimal(oldvalue) == false) oldvalue = 0;
    if (value == "" || IsDecimal(value) == false) value = 0;
    var netvalue = parseFloat(net.val()) - parseFloat(oldvalue) + parseFloat(value);
    net.val(netvalue);
    tax.val(((parseFloat(netvalue) * 15) * 0.01).toFixed(2));
    gross.val(((parseFloat(netvalue) * 115) * 0.01).toFixed(2));
}

function afterRowSave() {
    $.ajax({
        type: "POST",
        url: "receiptedit.aspx/GetGross",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#LabelGross").html(msg.d);
        }
    });
}


function ImageButtonDelete_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridTrans") + "_datagrid1");
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