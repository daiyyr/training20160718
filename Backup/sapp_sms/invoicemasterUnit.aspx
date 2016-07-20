<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="invoicemasterUnit.aspx.cs" Inherits="sapp_sms.invoicemasterUnit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Invoice Master</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/invoicemaster.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div>
            <ajaxToolkit:ComboBox ID="ComboBoxAllocated" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                AutoCompleteMode="SuggestAppend" CaseSensitive="False" Width="100px" 
                ItemInsertLocation="Append" Visible="False">
                <asp:ListItem Selected="True" Value="*">All</asp:ListItem>
                <asp:ListItem Value="0">Paid</asp:ListItem>
                <asp:ListItem Value="1">Unpaid</asp:ListItem>
            </ajaxToolkit:ComboBox>
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
        <div>
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridTable" colNames="['ID', 'Num','Debtor','Bodycorp','Unit', 'Date', 'Due', 'Gross', 'Paid','Balance']"
                colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                 { name: 'Num', index: 'Num', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Debtor', index: 'Debtor', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Bodycorp', index: 'Bodycorp', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Unit', index: 'Unit', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Date', index: 'Date', width: 150, align: 'left', search: true, formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}},
                 { name: 'Due', index: 'Due', width: 150, align: 'left', search: true, formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}},
                 { name: 'Gross', index: 'Gross', width: 150, align: 'left', search: false},
                 { name: 'Paid', index: 'Gross', width: 150, align: 'left', search: false},
                        { name: 'Balance', index: 'Gross', width: 150, align: 'left', search: false}
                 ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc"
                viewrecords="true" width="700" height="500" url="invoicemasterUnit.aspx/DataGridDataBind"
                hasID="false" />
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Make Receipt</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit0" runat="server" ImageUrl="Images/edit.gif"
                    OnClientClick="MakePaymentClick()" />
                <script>
                    function show(id) {
                        var url = "invoiceReceipt.aspx?invid=" + id;
                        vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 300px; dialogWidth: 450px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
                        if (vReturnValue == "refresh") {
                            __doPostBack('__Page', 'Refresh|0');
                        }
                    }
                    function MakePaymentClick() {

                        var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                        var rowKeys = grid.getGridParam("selrow");
                        if (rowKeys.length > 0) {
                            var id = grid.getCell(rowKeys, 'ID');
                            show(id);
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
    </div>
</asp:Content>
