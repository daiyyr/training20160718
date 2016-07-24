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
                var grid = jQuery("#" + GetClientId("jqGridRelated") + "_datagrid1");
                var rowKey = grid.getGridParam("selrow");
                var gross = grid.getCell(rowKey, 'Gross');

                if (!IsDecimal(value)) {
                    return [false, colname + 'should be decimal'];
                }
                else if (isNaN(value) == true || isFinite(value) == false) {
                    return [false, colname + ' should be decimal'];
                }
                else if (parseFloat(value) <= 0) {
                    return [false, colname + ' should be larger than zero'];
                }
                else if (gross != null && parseFloat(value) > parseFloat(gross)) {
                    return [false, colname + ' should be less than gross'];
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
        var refs = "";
        rowKeys = rowKeys + '';
        var keyss = rowKeys.split(',');
        $.each(keyss, function (n, value) {
            var rowID = grid.getCell(value, 'ID');
            ids = ids + rowID + ",";
        });
        $.each(keyss, function (n, value) {
            var rowREF = grid.getCell(value, 'RefType');
            refs = refs + rowREF + ",";
        });
        ids = ids.substring(0, ids.length - 1);
        refs = refs.substring(0, refs.length - 1);
        __doPostBack('__Page', 'ImageButtonUp|' + ids + "|" + refs);
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
        var ref = grid.getCell(rowKey, 'RefType');
        var DISC = grid.getCell(rowKey, 'Discount');
        __doPostBack('__Page', 'ImageButtonDown|' + ID + '|' + DISC + '|' + ref);
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
        type: "POST", url: "invoiceallocate.aspx/GetAllocate",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: AllocateResult
    };
    $.ajax(options);

}
function ChangeCreditNoteAllocate() {
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "creditnoteallocate.aspx/GetAllocate",
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

function ImageButtonClose_ClientClick() {
    var gross = document.getElementById(GetClientId("LabelGross")).innerHTML;
    var allocated = document.getElementById(GetClientId("LabelAllocated")).innerHTML;

    $("#HiddenOfferDiscount").val("false");

    if (gross != "" && allocated != "" && $("#HiddenHasReceipt").val() == "true") {
        var diff = parseFloat(gross) - parseFloat(allocated);

        if (diff > 0 && diff <= parseFloat($("#HiddenMaxDiscount").val())) {
            if (confirm('There is a small difference between "Gross" and "Allocated".\n Offer a discount?')) {
                $("#HiddenOfferDiscount").val("true");
                return true;
            }
        }
    }

    return true;
}

function DiscountOrNot(value, colname) {
        var grid = $('#jqGridRelated');
        if ($("#HiddenOfferDiscount").val() == "true") {
            return [true, ""];
        }
        else {
            if (value != 0 && (value != "" || value != null)) {
                return [false, ", Discount was not allowed in this Bodycorp"];
            }
            else {
                return [true, ""];
            }
        }
}
