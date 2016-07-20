

function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}
$(document).ready(function () {
    var txtAdminFee = jQuery("#" + GetClientId("TextBoxAdminFee"));
    txtAdminFee.hide();
    var checkUnit = jQuery("#" + GetClientId("CheckBoxUnitAdminFee"));
    checkUnit.click(function () {
        if (checkUnit.is(':checked')) {
            var txtAdminFee = jQuery("#" + GetClientId("TextBoxAdminFee"));
            txtAdminFee.show();
        }
        else {
            var txtAdminFee = jQuery("#" + GetClientId("TextBoxAdminFee"));
            txtAdminFee.hide();
        }
    });

});

function IsDecimal(value) {
    var regex = /^(\+|-)?([0-9]*\.?[0-9]*)$/;
    if (regex.test(value))
        return true;
    else
        return false;
}

function ChangeTotal() {
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "ccinvoiceedit.aspx/GetNetTotal",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalNetResult
    };
    $.ajax(options);
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "ccinvoiceedit.aspx/GetTaxTotal",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalTaxResult
    };
    $.ajax(options);
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "ccinvoiceedit.aspx/GetGrossTotal",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalGrossResult
    };
    $.ajax(options);
}

function TotalNetResult(response) {
    var total_net = response.d;
    var LabelNet = document.getElementById(GetClientId('LabelNet'));
    LabelNet.innerHTML = total_net;
}

function TotalTaxResult(response) {
    var total_tax = response.d;
    var LabelTax = document.getElementById(GetClientId('LabelTax'));
    LabelTax.innerHTML = total_tax;
}

function TotalGrossResult(response) {
    var total_gross = response.d;
    var LabelGross = document.getElementById(GetClientId('LabelGross'));
    LabelGross.innerHTML = total_gross;
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

function ValidateRowData(value, colname) {   
    if (value == "") {
        return [false, colname+' Required'];
    }
    else {
        if (colname == "Net(*)") {
            if (!IsDecimal(value)) {
                return [false, colname + 'should be decimal'];
            }
        }
        else if (colname == "Tax(*)") {
            if (!IsDecimal(value)) {
                return [false, colname + 'should be decimal'];
            }
        }
        else if (colname == "Gross(*)") {
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


function isGTSChecked() {
    if (document.getElementById('isGSTChecked').value == "Yes") {
        return "Yes";
    } else {
        return "No";
    }
}

