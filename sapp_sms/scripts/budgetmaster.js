function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}

var jsonVal = null;
$(document).ready(function () {
    var usedFlgs = $('#HiddenUsedFlgs').val();
    jsonVal = jQuery.parseJSON(usedFlgs);

    if ($('#HiddenCMTotalAmount').val() == $('#HiddenCMCurrentAmount').val()) {
        $('#ImageButtonAddRow').attr('disabled', 'disabled');
    }
})

function OnFillClick() {
    var fillValue = parseFloat($("#TextBoxValue").val()).toFixed(2);
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");
    if (rowKeys) {
        rowKeys = rowKeys + '';
        var keyss = rowKeys.split(',');

        $.each(keyss, function (n, value) {
            var rowKey = value;

            var is_done = false;
            var total = 0.0;
            for (i = 1; i <= 12; i++) {
                if (jsonVal != null && jsonVal[grid.getCell(rowKey, 'ID')] != null && jsonVal[grid.getCell(rowKey, 'ID')]["M" + i + "_Used"] == "1") {
                    //
                } else {
                    is_done = true;
                    grid.setCell(rowKey, "M" + i, fillValue);
                }
            }

            if (is_done == false) {
                alert("All the budget in this rows are used, cann't be changed.");
            }
        });

        SaveDataGrid();
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function OnSplitClick() {
    var totalvalue = parseFloat($("#TextBoxTotal").val()).toFixed(2);
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");
    if (rowKeys) {
        rowKeys = rowKeys + '';
        var keyss = rowKeys.split(',');

        $.each(keyss, function (n, value) {
            var rowKey = value;

            var counter = 0;
            var column_idx = new Array();
            for (i = 1; i <= 12; i++) {
                if (jsonVal != null && jsonVal[grid.getCell(rowKey, 'ID')] != null && jsonVal[grid.getCell(rowKey, 'ID')]["M" + i + "_Used"] == "1") {
                    // do nothing
                } else {
                    counter = counter + 1;
                    column_idx[counter] = i;
                }
            }

            if (counter == 0) {
                alert("All the budget in this rows are used, cann't be changed.");
            } else {
                var fillValue = parseFloat(totalvalue / counter).toFixed(2);

                for (i = 1; i < counter; i++) {
                    grid.setCell(rowKey, 'M' + column_idx[i], fillValue);
                }

                grid.setCell(rowKey, 'M' + column_idx[counter], (totalvalue - fillValue * (counter - 1)).toFixed(2));
            }
        });

        SaveDataGrid();
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}
function ImageButtonSave_ClientClick() {
    if (checkDuplicatedData() == false) {
        return false;
    }

    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rows = grid.getRowData();
    __doPostBack('__Page', 'ImageButtonSave|' + JSON.stringify(rows));
    return false;
}

function ButtonDelete_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rowKeys = grid.getGridParam("selarrrow");

    $('#ImageButtonAddRow').removeAttr('disabled');

    var ID = grid.getCell(grid.getGridParam("selrow"), 'ID');
    for (i = 1; i <= 12; i++) {
        if (jsonVal != null && jsonVal[ID] != null && jsonVal[ID]["M" + i + "_Used"] == "1") {
            alert("Selected row contains used budget, it cann't be deleted.");
            return false;
        }
    }

    if (confirm("Are you sure you want to delete the item?") == true) {

        if (rowKeys) {
            rowKeys = rowKeys + '';
            var keyss = rowKeys.split(',');
            $.each(keyss, function (n, value) {
                var rowKey = value;
                grid.delRowData(rowKey);
            });

            SaveDataGrid();
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

function SaveDataGrid() {
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rows = grid.getRowData();
    $('#HiddenCMCurrentAmount').val(rows.length);
    var json = JSON.stringify(rows) + '';
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "budgetmaster.aspx/SaveDataGrid",
        data: "{postdata:" + JSON.stringify(rows) + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false
    };
    $.ajax(options);
}

function ImageButtonDelete_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
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

function ImageButtonDetails_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rowKey = grid.getGridParam("selrow");

    if (rowKey) {
        var ID = grid.getCell(rowKey, 'ID');
        __doPostBack('__Page', 'ImageButtonDetails|' + ID);
        return false;
    }
    else {
        alert("Please select a row first!");
        return false;
    }

    return false;
}

function IsDecimal(value) {
    var regex = /^(\+|-)?([0-9]*\.?[0-9]*)$/;
    if (regex.test(value))
        return true;
    else
        return false;
}

function ValidateRowData(value, colname) {
    if (value == "") {
        return [true, ''];
    }
    else {
        if (!IsDecimal(value)) {
            return [false, ", the value in [" + colname + "] should be decimal."];
        }
        return [true, ''];
    }
}

function onCellBlur(e) {
    var hiddenCurValue = $('#HiddenCurValue').val();
    if (!IsDecimal(e.value) || !IsDecimal(hiddenCurValue)) {// When leave the cell, check the input value. If not decimal, go back
        e.value = hiddenCurValue;
    }
}

function setBackgroundColor(rowId, cellValue, rawObject, colModel, arrData) {
    var style = "";
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");

    if (jsonVal != null && rawObject != null && rawObject[0] != null && rawObject[0] != "" && jsonVal[rawObject[0]][colModel.name + "_Used"] != null && jsonVal[rawObject[0]][colModel.name + "_Used"] == "1") {
        style = " style='color: red; background-color: whitesmoke;' ";
    }

    return style;
}


function onRowClientSelect() {
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rowid = grid.getGridParam("selrow");
    var chart_master_id = grid.jqGrid('getCell', rowid, 'ID') ;

    var is_used = false;
    for (i = 1; i <= 12; i++) {
        if (jsonVal != null && jsonVal[chart_master_id] != null && jsonVal[chart_master_id]["M" + i + "_Used"] == "1") {
            grid.setColProp("M" + i, { editable: false });
            is_used = true;
        } else {
            grid.setColProp("M" + i, { editable: true });
        }
    }

    if (is_used == true) {
        grid.setColProp("Field", { editable: false });
        grid.setColProp("Scale", { editable: false });
    } else {
        grid.setColProp("Field", { editable: true });
        grid.setColProp("Scale", { editable: true });
    }

    var chart_master_id = grid.jqGrid('getCell', rowid, 'ID');
    grid.jqGrid('setColProp', 'Field', { editoptions: { dataUrl: 'budgetmaster.aspx/BindBudgetField?id=' + chart_master_id} });
}


function checkDuplicatedData() {
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");
    var rows = grid.getRowData();
    var cmIds = new Array();

    for (i = 0; i < rows.length; i++) {
        if (cmIds[rows[i].ID] != null && cmIds[rows[i].ID] != undefined) {
            alert("Chart master ID in row [" + cmIds[rows[i].ID] + "] and [" + (i + 1) + "] are duplicated. Please change to another ID.");
            return false;
        } else {
            cmIds[rows[i].ID] = i + 1;
        }
    }

    return true;
}

function ButtonAddRow_ClientClick() {
    var grid = jQuery("#" + GetClientId("jqGridBudget") + "_datagrid1");

    $('#HiddenCMCurrentAmount').val(parseInt($('#HiddenCMCurrentAmount').val()) + 1);
    grid.jqGrid('setColProp', 'Field', { editoptions: { dataUrl: 'budgetmaster.aspx/BindBudgetField?id='} });
    grid.setColProp("Field", { editable: true });
    grid.setColProp("Scale", { editable: true });
    for (i = 1; i <= 12; i++) {
        grid.setColProp("M" + i, { editable: true });
    }

    lastsel = 0;
    grid.jqGrid('addRow', "new");

    $('#' + GetClientId("jqGridBudget") + "_datagrid1_iledit").addClass("ui-state-disabled");
    $('#' + GetClientId("jqGridBudget") + "_datagrid1_ilsave").removeClass("ui-state-disabled");
    $('#' + GetClientId("jqGridBudget") + "_datagrid1_ilcancel").removeClass("ui-state-disabled");
    return false;
}
