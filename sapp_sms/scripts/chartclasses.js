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
        var grid = jQuery("#" + GetClientId("jqGridChartClasses") + "_datagrid1");
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
        return [true, ''];
    }
}
function ValidateBalanceSheet(value, colname) {
    if (value != "0" && value != "1") {
        return [false, " Balance Sheet value could only be either 0 or 1"]
    }
    else {
        return [true, ''];
    }
}