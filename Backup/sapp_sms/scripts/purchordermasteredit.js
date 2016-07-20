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
function ChangeNet(oldvalue, value) {
    var net = jQuery("#" + GetClientId("LabelNet"));
    var tax = jQuery("#" + GetClientId("LabelTax"));
    var gross = jQuery("#" + GetClientId("LabelGross"));
    if (net.text() == "") net.val(0);
    else net.val(net.text());
    if (oldvalue == "" || IsDecimal(oldvalue) == false) oldvalue = 0;
    if (value == "" || IsDecimal(value) == false) value = 0;
    var netvalue = parseFloat(net.val()) - parseFloat(oldvalue) + parseFloat(value);
    net.attr("innerHTML", netvalue);
    tax.attr("innerHTML", ((parseFloat(netvalue) * 15) * 0.01).toFixed(2));
    gross.attr("innerHTML", ((parseFloat(netvalue) * 115) * 0.01).toFixed(2));
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
