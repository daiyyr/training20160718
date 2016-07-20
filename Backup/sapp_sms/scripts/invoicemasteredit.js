function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}
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
        type: "POST", url: "invoicemasteredit.aspx/GetNetTotal",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalNetResult
    };
    $.ajax(options);
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "invoicemasteredit.aspx/GetTaxTotal",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalTaxResult
    };
    $.ajax(options);
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "invoicemasteredit.aspx/GetGrossTotal",
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
        return [false, colname + ' Required'];
    }
    else {
        if (colname == "Net(*)") {
            if (!IsDecimal(value)) {
                return [false, colname + ' should be decimal'];
            }
        }
        else if (colname == "Tax(*)") {
            if (!IsDecimal(value)) {
                return [false, colname + ' should be decimal'];
            }
        }
        else if (colname == "Gross(*)") {
            if (!IsDecimal(value)) {
                return [false, colname + ' should be decimal'];
            }


        }
        else {
            return [true, ''];
        }
        return [true, ''];
    }
}


function isCellEditable() {
    if (document.getElementById('isLimitedEdit').value == "true") {
        return false;
    } else {
        return true;
    }
}


function isGTSChecked() {
    if (document.getElementById('isGSTChecked').value == "Yes") {
        return "Yes";
    } else {
        return "No";
    }
}


function searchChartData() {
    var chartList = document.getElementById('hidenChartCodeName').value;
    var jsonVal = jQuery.parseJSON(chartList);

    var chartText = document.getElementById('TxtChartName').value;
    var reach_regex = new RegExp(".*" + chartText.toUpperCase() + ".*", "g");

    var findCode = false;
    var resultContent = "";
    $('#LblChartMasterSearchResult').html("");

    jQuery.each(jsonVal, function (i, val) {
        if (val.chart_master_name.toUpperCase().match(reach_regex)) {
            findCode = true;
            resultContent = resultContent + "<tr> <td>" + val.chart_master_code + "</td> <td>" + val.chart_master_name + " </td> </tr>";
        }
    });

    if (findCode == false) {
        alert("Cann't find any Chart of Account code, please try other name.");
    } else {
        resultContent = "<div style='width:588px; max-height:160px; overflow-y:scroll;'><table cellpadding='0' cellspacing='0' border='1'><thead><tr style='background-color: lightblue;'> <td width='160'>Code</td>  <td width='400'>Name</td> </tr></thead> <tbody>" + resultContent + "</tbody></table></div>";
        $('#LblChartMasterSearchResult').html(resultContent);
    }

    return false;
}


function OnInvoiceDateChange() {
    var invVal = document.getElementById('TextBoxDate').value;

    if (invVal.length == 10) {
        var dueDate = new Date(invVal.substr(6, 4), invVal.substr(3, 2), invVal.substr(0, 2), 0, 0, 0);
        dueDate.setMonth(dueDate.getMonth());

        if (document.getElementById('HiddenFieldBaseDateBkn').value == "InvoiceDate") {
            document.getElementById('TextBoxDue').value = dueDate.format("dd/MM/yyyy");
            document.getElementById('TextBoxApply').value = invVal;
        } else {
            document.getElementById('TextBoxDue').value = dueDate.format("dd/MM/yyyy");
            document.getElementById('TextBoxApply').value = dueDate.format("dd/MM/yyyy");
        }
    } else {
        document.getElementById('TextBoxDue').value = "";
        document.getElementById('TextBoxApply').value = "";
    }

    return false;
}


function OnDueDateChange() {


    if (document.getElementById('HiddenFieldBaseDateBkn').value == "DueDate") {
        document.getElementById('TextBoxApply').value = document.getElementById('TextBoxDue').value;
    }

    return false;
}
