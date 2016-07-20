<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="activity.aspx.cs" Inherits="sapp_sms.activity" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bodycorp Activity</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/activity.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false" OnClientClick="history.back(); return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div id="d1">
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridActivity" colNames="['ID', 'Date','Ref','Description','Income','Expense','Deposit','Payment','Journal','Type','Rec','Rev']"
                colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', hidden:true},
                  { name: 'Date', index: 'Date', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Ref', index: 'Ref', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Income', index: 'Income', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Expense', index: 'Expense', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Deposit', index: 'Deposit', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Payment', index: 'Payment', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Journal', index: 'Journal', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Type', index: 'Type', width: 50,editable:false, align: 'left', sorttype: 'int', hidden:true},
                  { name: 'Rec', index: 'Rec', width: 50,editable:false, align: 'left', sorttype: 'int' },
                  { name: 'Rev', index: 'Rev', width: 50,editable:false, align: 'left', sorttype: 'int' }
              ]" rowNum="500" rowList="[100, 200, 500, 1000, 5000]" sortname="Date" sortorder="asc"
                viewrecords="true" width="800" height="500" url="activity.aspx/jqGridActivityDataBind"
                hasID="false" footerrow="true" userDataOnFooter="true" />
            <div style="background-color: White">
            </div>
        </div>
        <div id="d2" style="width: 200px;">
            <table class="details">
                <tr>
                    <td>
                        Date:&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="AcDateT" CssClass="d1" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate" CssClass="sappcalendar"
                            Format="dd/MM/yyyy" TargetControlID="AcDateT" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="SubmitB" runat="server" Text="Submit" OnClientClick="return AcReverseClick()" />
                        <asp:Button ID="CancelB" runat="server" Text="Cancel" OnClientClick="return Cancel()" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Detail</div>
            <div>
                <asp:ImageButton ID="DetailB" runat="server" ImageUrl="Images/detail.gif" CausesValidation="false"
                    OnClientClick="return DetailClick()" />
            </div>
            <div class="button-title">
                Reverse</div>
            <div>
                <asp:ImageButton ID="ReverseIB" runat="server" ImageUrl="Images/transfer.gif" CausesValidation="false"
                    OnClientClick="return ReverseClick()" />
            </div>
            <div class="button-title">
                Rev Accrual</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/transfer.gif" CausesValidation="false"
                    OnClientClick="return ShowAC()" />
            </div>
            <script>
                $("#d2").hide();
                function show(id, type) {
                    var url = "activityunitdetail.aspx?id=" + id + "&type=" + type;
                    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
                    if (vReturnValue == "refresh") {
                        __doPostBack('__Page', 'Refresh|0');
                    }
                }
                function DetailClick() {

                    var grid = jQuery("#" + GetClientId("jqGridActivity") + "_datagrid1");
                    var rowKeys = grid.getGridParam("selrow");
                    if (rowKeys.length > 0) {
                        var id = grid.getCell(rowKeys, 'ID');
                        var t = grid.getCell(rowKeys, 'Type');
                        show(id, t);
                    }
                    else {
                        alert("Please select a row first!");
                        return false;
                    }
                    return false;
                }
                function ShowAC() {
                    var grid = jQuery("#" + GetClientId("jqGridActivity") + "_datagrid1");
                    var rowKeys = grid.getGridParam("selrow");
                    if (rowKeys.length > 0) {
                        var id = grid.getCell(rowKeys, 'ID');
                        var t = grid.getCell(rowKeys, 'Type');
                        if (t == "Journal") {
                            $("#d2").show();
                            $("#d1").hide();
                        }
                    }
                    else {
                        alert("Please select a row first!");
                        return false;
                    }
                    return false;
                }
                function Cancel() {
                    $("#d2").hide();
                    $("#d1").show();
                }
                function AcReverseClick() {
                    var grid = jQuery("#" + GetClientId("jqGridActivity") + "_datagrid1");
                    var rowKeys = grid.getGridParam("selrow");
                    if (rowKeys.length > 0) {
                        var id = grid.getCell(rowKeys, 'ID');
                        var t = grid.getCell(rowKeys, 'Type');
                        if (t == "Journal")
                            if (confirm("Reverse this transaction?")) {
                                ACReverse(id, t);
                            }
                    }
                    else {
                        alert("Please select a row first!");
                        return false;
                    }
                    return false;
                }
                function ReverseClick() {
                    var grid = jQuery("#" + GetClientId("jqGridActivity") + "_datagrid1");
                    var rowKeys = grid.getGridParam("selrow");
                    if (rowKeys.length > 0) {
                        var id = grid.getCell(rowKeys, 'ID');
                        var t = grid.getCell(rowKeys, 'Type');
                        var rec = grid.getCell(rowKeys, 'Rec');
                        if (t == "Journal" && rec != "Rec")
                            if (confirm("Reverse this transaction?")) {
                                Reverse(id, t);
                            }
                    }
                    else {
                        alert("Please select a row first!");
                        return false;
                    }
                    return false;
                }
                function ACReverse(id, type) {
                    var url = "activityunitdetail.aspx?id=" + id + "&type=" + type + "&op=reverse&date=" + $(".d1").val();
                    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; ");
                    if (vReturnValue == "refresh") {
                        __doPostBack('__Page', 'Refresh|0');
                    }
                }
                function Reverse(id, type) {
                    var url = "activityunitdetail.aspx?id=" + id + "&type=" + type + "&op=reverse";
                    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; ");
                    if (vReturnValue == "refresh") {
                        __doPostBack('__Page', 'Refresh|0');
                    }
                }

            </script>
        </div>
    </div>
</asp:Content>
