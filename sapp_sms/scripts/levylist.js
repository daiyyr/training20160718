function GetClientId(serverId) {
    for (i = 0; i < MyServerID.length; i++) {
        if (MyServerID[i] == serverId) {
            return MyClientID[i];
            break;
        }
    }
}



function ChangeTotal() {
    var options = {
        error: function (response) { var r = jQuery.parseJSON(response.responseText); alert("ExceptionType: " + r.ExceptionType + " \r\nMessage: " + r.Message); },
        type: "POST", url: "levylist.aspx/GetTotal",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: TotalResult
    };
    $.ajax(options);

}

function TotalResult(response) {
    var total = response.d;
    var labelTotalAmount = document.getElementById(GetClientId('LabelTotalAmount'));
    labelTotalAmount.innerHTML = total;
}

function ImageButtonDelete_ClientClick() {
    if (confirm("Are you sure you want to delete the item?") == true) {
        var grid = jQuery("#" + GetClientId("jqGridLevyList") + "_datagrid1");
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

function ButtonOKClick() {
    __doPostBack('__Page', 'ButtonOK|1');
    return false;
}