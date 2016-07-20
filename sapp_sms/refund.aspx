<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="refund.aspx.cs" Inherits="sapp_sms.refund" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Refund</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/receipts.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
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
                    PostBackUrl="~/receipts.aspx" OnClientClick="return ImageButtonDetails_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="Images/edit.gif" PostBackUrl="~/receipts.aspx"
                    OnClientClick="return ImageButtonEdit_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="Images/delete.gif"
                    PostBackUrl="~/receipts.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
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
            <cc1:jqGridAdv runat="server" ID="jqGridTable" colNames="['receipt_id', 'Unit','Bodycorp', 'Debtor', 'Ref',  'Gross','Allocated','Balance','Date']"
                colModel="[{ name: 'receipt_id', index: 'receipt_id', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['cn', 'nc']} , hidden:true},
                { name: 'Unit', index: 'unit_master_code', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Bodycorp', index: 'bodycorp_code', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Debtor', index: 'debtor_master_name', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Ref', index: 'receipt_ref', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Gross', index: 'receipt_gross', width: 150, align: 'right', formatter: 'number',formatoptions:{thousandsSeparator: ''}, search: true,searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Allocated', index: 'receipt_allocated', width: 150, align: 'right', formatter: 'number',formatoptions:{thousandsSeparator: ''}, search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Balance', index: 'Balance', width: 150, align: 'right', formatter: 'number',formatoptions:{thousandsSeparator: ''}, search: true, searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Date', index: 'receipt_date', width: 100, align: 'left', formatter: 'date',formatoptions:{srcformat: 'd/m/Y'}, search: true, searchoptions: { sopt: ['cn', 'nc']}}
                 ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="receipt_date" sortorder="desc"
                viewrecords="true" width="700" height="500" url="refund.aspx/DataGridDataBind"
                hasID="false" />
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Allocate</div>
            <div>
                <asp:ImageButton ID="ImageButtonAllocate" runat="server" ImageUrl="Images/edit.gif"
                    PostBackUrl="~/refund.aspx" OnClientClick="return ImageButtonAllocate_ClientClick()" />
            </div>
        </div>
    </div>
</asp:Content>
