<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="journalList.aspx.cs" Inherits="sapp_sms.journalList" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Journal List</title>
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
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" 
                OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" Enabled="False">
                <asp:ListItem>ALL</asp:ListItem>
                <asp:ListItem Value="JNL">Journal</asp:ListItem>
                <asp:ListItem Value="CD">Cash Deposit</asp:ListItem>
                <asp:ListItem Value="CP">Cash Payment</asp:ListItem>
            </asp:DropDownList>
            <div class="button-title">
                Add</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/detail.gif" CausesValidation="false"
                    OnClick="ImageButton2_Click" />
            </div>
<%--            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="Images/detail.gif" CausesValidation="false"
                    OnClientClick="return EditClick()" />
            </div>--%>
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
            <%--<div class="button-title">
                Rev Accrual</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/transfer.gif"
                    CausesValidation="false" OnClientClick="return ShowAC()" Width="32px" />
            </div>--%>
            <script type="text/javascript">
                $("#d2").hide();
                function show(id, type) {
                    var url = "activityunitdetail.aspx?id=" + id + "&type=" + type;
                    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
                    if (vReturnValue == "refresh") {
                        __doPostBack('__Page', 'Refresh|0');
                    }
                }
                function EditClick() {
                    var grid = jQuery("#" + GetClientId("jqGridActivity") + "_datagrid1");
                    var rowKeys = grid.getGridParam("selrow");
                    if (rowKeys.length > 0) {
                        var jref = grid.getCell(rowKeys, 'Ref');
                        var url = "journalsedit.aspx?jref=" + jref + "&mode=bc";
                        var vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;");

                        __doPostBack('__Page', 'Refresh|0');
                    }
                    else {
                        alert("Please select a row first!");
                        return false;
                    }
                    return false;
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
                        if (t == "Journal")
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
            <cc1:jqGridAdv runat="server" ID="jqGridActivity" colNames="['ID', 'Date','Ref','Description','Journal','Type','Rec','Rev']"
                colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', hidden:true},
                  { name: 'Date', index: 'Date', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Ref', index: 'Ref', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Journal', index: 'Journal', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Type', index: 'Type', width: 50,editable:false, align: 'left', sorttype: 'int', hidden:true},
                  { name: 'Rec', index: 'Rec', width: 50,editable:false, align: 'left', sorttype: 'int' },
                  { name: 'Rev', index: 'Rev', width: 50,editable:false, align: 'left', sorttype: 'int' }
              ]" rowNum="500" rowList="[100, 200, 500, 1000, 5000]" sortname="Date" sortorder="asc"
                viewrecords="true" width="800" height="500" url="journalList.aspx/jqGridActivityDataBind"
                hasID="false" footerrow="true" userDataOnFooter="true" />
            <div style="background-color: White">
            </div>
        </div>
    </div>
    <div id="content_right">
    <div id="content_right0">
        <%--        <div class="button">
            <div class="button-title">
                Export</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClick="ImageButton2_Click" />
            </div>
        </div>--%>
        <div class="button">
            <div class="button-title">
                Import</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClick="ImageButton1_Click" />
            </div>
            <div class="button-title">
                Template</div>
            <div>
                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/images/history.gif"
                    CausesValidation="false" OnClick="ImageButton4_Click" />
            </div>
        </div>
    </div>
    </div>
</asp:Content>
