
function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}


$(document).ready(function () {
    if (document.getElementById('CheckBoxStmt').checked) {
        document.getElementById('TextBoxStmtStartDate').disabled = false;
        document.getElementById('TextBoxStmtEndDate').disabled = false;
    } else {
        document.getElementById('TextBoxStmtStartDate').value = "";
        document.getElementById('TextBoxStmtEndDate').value = "";
        document.getElementById('TextBoxStmtStartDate').disabled = true;
        document.getElementById('TextBoxStmtEndDate').disabled = true;
    }

    if (document.getElementById('CheckBoxNotes').checked) {
        document.getElementById('FileUploadNotes').disabled = false;
    } else {
        document.getElementById('FileUploadNotes').disabled = true;
    }

    $('#CheckBoxStmt').click(function () {
        if ($(this).is(':checked')) {
            document.getElementById('TextBoxStmtStartDate').disabled = false;
            document.getElementById('TextBoxStmtEndDate').disabled = false;
        } else {
            document.getElementById('TextBoxStmtStartDate').value = "";
            document.getElementById('TextBoxStmtEndDate').value = "";
            document.getElementById('TextBoxStmtStartDate').disabled = true;
            document.getElementById('TextBoxStmtEndDate').disabled = true;
        }
    });

    $('#CheckBoxNotes').click(function () {
        if ($(this).is(':checked')) {
            document.getElementById('FileUploadNotes').disabled = false;
        } else {
            document.getElementById('FileUploadNotes').disabled = true;
        }
    });

})


function ImageButtonNewReport_ClientClick() {
    if (document.getElementById('CheckBoxStmt').checked) {
        if (document.getElementById('TextBoxStmtStartDate').value == '') {
            alert("Please select start date for statement report.");
            return false;
        }
        if (document.getElementById('TextBoxStmtEndDate').value == '') {
            alert("Please select end date for statement report.");
            return false;
        }
    }

    if (document.getElementById('CheckBoxNotes').checked) {
        var notesFile = document.getElementById('FileUploadNotes').value;
        if (notesFile == '') {
            alert("Please select a notes file.");
            return false;
        } else if (notesFile.endsWith(".pdf") == false) {
            alert("Please select a pdf file as notes.");
            return false;
        }
    }

    var grid = jQuery("#" + GetClientId("jqGridInvoice") + "_datagrid1");
    var rowIdxs = grid.getGridParam("selarrrow");

    if (rowIdxs.length > 0) {
        var Idxs = rowIdxs.toString().split(',');
        var invoiceIdList = new Array();

        $.each(Idxs, function (n, value) {
            invoiceIdList.push(grid.getCell(value, 'ID'));
        });

        __doPostBack('__Page', 'ImageButtonNewReport|' + JSON.stringify(invoiceIdList));
    } else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}


function ImageButtonSendEmail_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridInvoice") + "_datagrid1");
    var rowIdxs = grid.getGridParam("selarrrow");

    if (document.getElementById('TextBoxSubject').value == "") {
        alert("Please edit email subject.");
        return false;
    }
    if (document.getElementById('TextBoxBody').value == "") {
        alert("Please edit email body.");
        return false;
    }

    if (rowIdxs.length > 0) {
        var Idxs = rowIdxs.toString().split(',');
        var args = "";

        $.each(Idxs, function (n, value) {
            var rowIdx = value;
            args = args + "|" + grid.getCell(rowIdx, 'ID');
        });

        __doPostBack('__Page', "ImageButtonSendEmail" + args);
        return false;
    } else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}


function setSelectedRow(rowId, cellValue, rawObject, colModel, arrData) {
    var grid = jQuery("#" + GetClientId("jqGridInvoice") + "_datagrid1");
    var rowArray = grid.getRowData();

    for (i = 0, count = rowArray.length; i < count; i += 1) {
        $grid.jqGrid('setSelection', rowArray[i], true);
    }
}
