function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

$(document).ready(function () {
    var $td = $('#' + GetClientId("jqGridRelated") + "_datagrid1_iladd");
    $td.hide();
});

function ValidateRowData(value, colname) {
    try {
        if (value == "") {
            return [false, colname + ' Required'];
        }
        else {
            if (colname == "Paid") {
                if (!IsDecimal(value)) {
                    return [false, colname + 'should be decimal'];
                }
                else {
                    return [true, ''];
                }
            }
            else {
                return [true, ''];
            }
            return [true, ''];
        }
    } catch (err) {
        alert(err.Message);
    }
}

function ImageButtonUp_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridUnpaid") + "_datagrid1");
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
    var grid = jQuery("#" + GetClientId("jqGridRelated") + "_datagrid1");
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

function ChangeAllocate() {
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "ccrefundlocate.aspx/GetAllocate",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: AllocateResult
    };
    $.ajax(options);
}

function AllocateResult(response) {
    var allocate = response.d;
    var LabelAllocated = document.getElementById(GetClientId('LabelAllocated'));
    LabelAllocated.innerHTML = allocate;
}