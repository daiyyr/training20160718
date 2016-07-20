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

function confirm_delete() {
    if (confirm("Are you sure you want to delete the item?") == true)
        return true;
    else
        return false;
}

function ButtonTranOSOK() {
}

function ButtonActivityOKClick() {
    var unit_id = $("#HiddenUnitId").val();
    var startdate = $("#TextBoxActivityStart").val();
    var enddate = $("#TextBoxActivityEnd").val();
    window.location = "activityunit.aspx?mode=unit&id=" + unit_id + "&start=" + startdate + "&end=" + enddate;
}

function ButtonDeleteAcc_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridAccUnit") + "_datagrid1");
        var rowKey = grid.getGridParam("selrow");

        if (rowKey) {
            var ID = grid.getCell(rowKey, 'ID');
            __doPostBack('__Page', 'ButtonDeleteAcc|' + ID);
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

var validate_code = "";
function ValidateCode(value, colname) {

    if (value == "") {
        return [false, colname + ' Required'];
    }
    else if (value.indexOf("\"") != -1) {
        return [false, colname + ' No \" Allowed!'];
    }
    else {
        var grid = jQuery("#" + GetClientId("jqGridUnitPlan") + "_datagrid1");
        var rowKey = grid.getGridParam("selrow");

        if (rowKey)
            var ID = grid.getCell(rowKey, 'ID');
        var options = {
            error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
            type: "POST", url: "unitmasterdetails.aspx/ValidateCode",
            data: "{postdata:'{\"code\":\"" + value + "\", \"id\":\"" + ID + "\"}'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: ValidateCodeResult
        };
        $.ajax(options);
        if (validate_code == "true")
            return [true, ''];
        else
            return [false, colname + validate_code];
    }
}

function ValidateCodeResult(response) {
    validate_code = response.d;
}

function ImageButtonTranOS_ClientClick() {
    var unitmasterid = querySt("unitmasterid");

    var url = "ownershiptransfer.aspx?unitmasterid=" + unitmasterid;
    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 1050px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
    if (vReturnValue == "refresh") {
        __doPostBack('__Page', 'Refresh|0');
    }

}