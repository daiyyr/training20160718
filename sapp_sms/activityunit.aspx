<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="activityunit.aspx.cs" Inherits="sapp_sms.activityunit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Activity</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/activityunit.js" type="text/javascript"></script>
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
        <div>
            <img src="Images/dot.gif" height="4px" />
            <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
                <li class="ui-state-default ui-corner-top">
                    <div id="titletabs">
                        <asp:Label ID="Label1" runat="server"></asp:Label>
                    </div>
                </li>
            </ul>
        </div>
        <cc1:jqGridAdv runat="server" ID="jqGridActivityUnit" colNames="['ID', 'Inv Date','Due Date','Ref','Description','Invoice','Receipt','Balance','Journal','Type','Rec','Rev']"
            colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', hidden:true},
                  { name: 'InvDate', index: 'InvDate', width: 100,  align: 'left', search: true, formatter:'date', formatoptions:{srcformat: 'd/m/Y'},  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'DueDate', index: 'DueDate', width: 100,  align: 'left', search: true, formatter:'date',formatoptions:{srcformat: 'd/m/Y'},  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Ref', index: 'Ref', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Invoice', index: 'Invoice', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Receipt', index: 'Receipt', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                                  
                  { name: 'Balance', index: 'Balance', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Journal', index: 'Journal', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Type', index: 'Type', width: 50,editable:false, align: 'left',  hidden:true },
                  { name: 'Rec', index: 'Rec', width: 50,editable:false, align: 'left', sorttype: 'int' },
                  { name: 'Rev', index: 'Rev', width: 50,editable:false, align: 'left', sorttype: 'int' }
              ]" rowNum="500" rowList="[100, 200, 500, 1000, 5000]" viewrecords="true" width="810"
            height="500" url="activityunit.aspx/jqGridAUDataBind"
            hasID="false" footerrow="true" userDataOnFooter="true" />
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Detail</div>
            <div>
                <asp:ImageButton ID="DetailB" runat="server" ImageUrl="Images/detail.gif" CausesValidation="false"
                    OnClientClick="return DetailClick()" onclick="DetailB_Click" />
            </div>
            <div class="button-title">
                Reverse</div>
            <div>
                <asp:ImageButton ID="ReverseIB" runat="server" ImageUrl="Images/transfer.gif" CausesValidation="false"
                    OnClientClick="return ReverseClick()" />
            </div>
            <div class="button">
                <div class="button-title">
                    Export</div>
                <div>
                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/export_pdf.gif"
                        OnClick="ImageButton2_Click" />
                </div>
            </div>
            <script>
                function show(id, type) {
                    var url = "activityunitdetail.aspx?id=" + id + "&type=" + type;
                    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
                    if (vReturnValue == "refresh") {
                        __doPostBack('__Page', 'Refresh|0');
                    }
                }
                function Reverse(id, type) {
                    var url = "activityunitdetail.aspx?id=" + id + "&type=" + type + "&op=reverse";
                    vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
                    if (vReturnValue == "refresh") {
                        __doPostBack('__Page', 'Refresh|0');
                    }
                }
                function DetailClick() {

                    var grid = jQuery("#" + GetClientId("jqGridActivityUnit") + "_datagrid1");
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
                function ReverseClick() {
                    var grid = jQuery("#" + GetClientId("jqGridActivityUnit") + "_datagrid1");
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

            </script>
        </div>
    </div>
</asp:Content>
