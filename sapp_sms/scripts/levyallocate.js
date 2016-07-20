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

function OnFillClick() {
    var installment = querySt('installment');

    var grid = jQuery("#" + GetClientId("jqGridLevy") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");
    if (rowKeys) {
        rowKeys = rowKeys + '';
        var keyss = rowKeys.split(',');

        $.each(keyss, function (n, value) {
            var rowKey = value;
            var totalValue = grid.getCell(rowKey, 'Total');
            var cellValue = parseFloat(totalValue) / parseInt(installment);

            for (i = 1; i < (parseInt(installment) + 1); i++) {
                grid.setCell(rowKey, 'M' + i, (cellValue).toFixed(2));
            }
        });
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function ImageButtonSave_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridLevy") + "_datagrid1");
    var rows = grid.getRowData();
    __doPostBack('__Page', 'ImageButtonSubmit|' + JSON.stringify(rows));
    return false;
}

$(window).load(function () {
    var installment = querySt('installment');
    var grid = jQuery('#' + GetClientId('jqGridLevy') + '_datagrid1');
    for (i = 1; i < (parseInt(installment) + 1); i++) {
        grid.showCol('M' + i);

    }
});


function OnBillingDateChange() {
    if (document.getElementById('HiddenFieldBaseDateBkn').value == "InvoiceDate") {
        document.getElementById('TextBoxApplyDateM').value = "0";
        document.getElementById('TextBoxApplyDateD').value = document.getElementById('TextBoxBillingDate').value;
    }

    return false;
}


function OnDueDateChange() {
    if (document.getElementById('HiddenFieldBaseDateBkn').value == "DueDate") {
        document.getElementById('TextBoxApplyDateM').value = document.getElementById('TextBoxDueM').value;
        document.getElementById('TextBoxApplyDateD').value = document.getElementById('TextBoxDueD').value;
    }

    return false;
}

