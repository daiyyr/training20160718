function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}
function ChangeTotal() {
    var options = {
        error: function (msg) { alert(msg.d+"dd"); },
        type: "POST", url: "unitplaninit.aspx/GetTotalOI",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (response) { var results = response.d; $("#LabelTotalOI").text(results); }
    };
    $.ajax(options);
    var options = {
        error: function (msg) { alert(msg.d + "dd"); },
        type: "POST", url: "unitplaninit.aspx/GetTotalUI",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (response) { var results = response.d; $("#LabelTotalUI").text(results); }
    };
    $.ajax(options);

}


function ImageButtonDelete_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridUnitPlan") + "_datagrid1");
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


function IsDecimal(value, colname) {
    var regex = /^(\+|-)?([0-9]*\.?[0-9]*)$/;
    if (regex.test(value))
        return true;
    else
        return false;
}
function DecimalNull(value, colname) {
    if (value == "") {
        return [true, ''];
    }
    else {
        if (IsDecimal(value, colname)) {
            return [true, ''];
        }
        else {
            return [false, colname+' should be decimal'];
        }
    }
}
function DecimalNotNull(value, colname) {
    if (value == "") {
        return [false, colname + ' Required'];
    }
    if (IsDecimal(value, colname)) {
        return [true, ''];
    }
    else {
        return [false, colname + ' should be decimal']; 
    }
}
function ValidateRowData(value, colname) {
    if (value == "") {
        return [false, colname + ' Required'];
    }
    else {
        return [true, ''];
    }
}