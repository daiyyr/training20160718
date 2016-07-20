<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="cpayments.aspx.cs" Inherits="sapp_sms.cpayments" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - CPayments</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/cpayments.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Allocated</div>
            <div>
                <ajaxToolkit:ComboBox ID="ComboBoxAllocated" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                    AutoCompleteMode="SuggestAppend" CaseSensitive="False" Width="100px" ItemInsertLocation="Append"
                    OnSelectedIndexChanged="ComboBoxAllocated_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Value="*">All</asp:ListItem>
                    <asp:ListItem Value="1">Allocated</asp:ListItem>
                    <asp:ListItem Value="0">Unallocated</asp:ListItem>
                </ajaxToolkit:ComboBox>
            </div>
            <div>
                <asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
            </div>
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
                    PostBackUrl="~/cpayments.aspx" OnClientClick="return ImageButtonDetails_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="Images/edit.gif" PostBackUrl="~/cpayments.aspx"
                    OnClientClick="return ImageButtonEdit_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="Images/delete.gif"
                    PostBackUrl="~/cpayments.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
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
            <cc1:jqGridAdv runat="server" ID="jqGridTable" colNames="['ID','Date', 'BodyCorp', 'Creditor','Type','Reference','Gross', 'Allocated', 'Balance']"
                colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
              { name: 'Date', index: 'Date', width: 150, align: 'left', search: true, formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}},
                 { name: 'BodyCorp', index: 'BodyCorp', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Creditor', index: 'Creditor', width: 100, align: 'left',  search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Type', index: 'Type', width: 100, align: 'left',  search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Reference', index: 'Reference', width: 150, align: 'left',  search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Gross', index: 'Gross', width: 100, align: 'right', formatter: 'number',formatoptions:{thousandsSeparator: ''}, search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Allocated', index: 'Allocated', width: 100, align: 'right', formatter: 'number',formatoptions:{thousandsSeparator: ''}, search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Balance', index: 'Balance', width: 100, align: 'right', formatter: 'number',formatoptions:{thousandsSeparator: ''}, search: true, searchoptions: { sopt: ['cn', 'nc']}}
                 ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Date" sortorder="desc"
                viewrecords="true" width="800" height="500" url="cpayments.aspx/DataGridDataBind"
                hasID="false" />
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Allocate</div>
            <div>
                <asp:ImageButton ID="ImageButtonAllocate" runat="server" ImageUrl="Images/edit.gif"
                    PostBackUrl="~/cpayments.aspx" OnClientClick="return ImageButtonAllocate_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Import</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/edit.gif" OnClick="ImageButton1_Click" />
            </div>
        </div>
    </div>
</asp:Content>
