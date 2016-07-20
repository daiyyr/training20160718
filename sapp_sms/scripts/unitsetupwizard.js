

function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
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

function ChangeTotal() {
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "unitsetupwizard.aspx/GetTotalOI",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalOIResult
    };
    $.ajax(options);
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "unitsetupwizard.aspx/GetTotalUI",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalUIResult
    };
    $.ajax(options);

}

function TotalOIResult(response) {
    var total_oi = response.d;
    var labelTotalOI = document.getElementById(GetClientId('LabelTotalOI'));
    labelTotalOI.innerHTML = total_oi;
}

function TotalUIResult(response) {
    var total_ui = response.d;
    var labelTotalUI = document.getElementById(GetClientId('LabelTotalUI'));
    labelTotalUI.innerHTML = total_ui;
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
            type: "POST", url: "unitsetupwizard.aspx/ValidateCode",
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

function ValidatePrinciple(value, colname) {


    var dropdownlists = document.getElementsByTagName("select");
    var dropdownlist = dropdownlists.Type;
    var index = dropdownlist.selectedIndex;
    var typecode = dropdownlist.options[index].text;
        
    if (typecode == "ACCESSORIES") {
        if (value == "" || value == "0") {
            return [false, colname + ' Required'];
        }
        else return [true, ''];
    }
    else {
        if (value == "" || value == "0") {
            return [true, ''];
        }
        else {
            return [false, colname + ' Not Required'];
        }
    }

}
