<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="invoicemasterOLD.aspx.cs" Inherits="sapp_sms.invoicemasterOLD" %>

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
        <div class="button-title">
            Paid</div>
        <div>
            <ajaxToolkit:ComboBox ID="ComboBoxAllocated" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                AutoCompleteMode="SuggestAppend" CaseSensitive="False" Width="100px" ItemInsertLocation="Append">
                <asp:ListItem Selected="True" Value="*">All</asp:ListItem>
                <asp:ListItem Value="0">Paid</asp:ListItem>
                <asp:ListItem Value="1">Unpaid</asp:ListItem>
            </ajaxToolkit:ComboBox>
        </div>
        <div>
            <asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
        </div>
        <div class="button">
            <div class="button-title">
                Add</div>
            <div>
                <asp:ImageButton ID="ImageButtonAdd" runat="server" ImageUrl="Images/new.gif" OnClick="ImageButtonAdd_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Details</div>
            <div>
                <asp:ImageButton ID="ImageButtonDetails" runat="server" ImageUrl="Images/detail.gif"
                    PostBackUrl="~/invoicemaster.aspx" OnClientClick="return ImageButtonDetails_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="Images/edit.gif" PostBackUrl="~/invoicemaster.aspx"
                    OnClientClick="return ImageButtonEdit_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="Images/delete.gif"
                    PostBackUrl="~/invoicemaster.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
            </div>
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
                 { name: 'Paid', index: 'Paid', width: 150, align: 'left', search: false},
                 { name: 'Balance', index: 'Balance', width: 150, align: 'left', search: false}
                 ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc"
                viewrecords="true" width="700" height="500" url="invoicemasterOLD.aspx/DataGridDataBind"
                hasID="false" />
        </div>
    </div>
    <div id="content_right">
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
        </div>
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
        <div class="button">
            <div class="button-title">
                Batch</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/edit.gif" OnClientClick="return BatchClick()" />
                <script>

                    function BatchClick() {
                        var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                        var rowKey = grid.getGridParam("selrow");

                        if (rowKey) {
                            var ID = grid.getCell(rowKey, 'ID');
                            __doPostBack('__Page', 'Batch|' + ID);
                            return false;
                        }
                        else {
                            alert("Please select a row first!");
                            return false;
                        }

                        return false;
                    }
                </script>
                <div class="button-title">
                    Allocate</div>
                <div>
                    <asp:ImageButton ID="ImageButtonAllocate" runat="server" 
                        ImageUrl="Images/edit.gif" 
                        OnClientClick="return ImageButtonAllocate_ClientClick()" 
                        PostBackUrl="~/receipts.aspx" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
