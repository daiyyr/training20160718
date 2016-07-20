<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="cpaymentallocate.aspx.cs" Inherits="sapp_sms.cpaymentallocate" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Cpayment Allocate</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/cpaymentallocate.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">Save</div>
            <div>
                <asp:ImageButton ID="ImageButtonSave"
                    runat="server" ImageUrl="Images/save.gif" onclick="ImageButtonSave_Click"/>
            </div>
        </div>
        <div  class="button">
        <div class="button-title">Cancel</div>
        <div>
            <asp:ImageButton ID="ImageButtonClose"
                runat="server" ImageUrl="Images/close.gif"  CausesValidation="false"  OnClientClick="history.back(); return false;"/>
        </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
         <tr>
            <td><b>Cpayment:<asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <b><asp:Label ID="LabelCpaymentID" runat="server" Text="ID"  Visible="false"></asp:Label></b>
            </td>
            <td><b>Date :</b></td>
            <td>
                <asp:Label ID="LabelDate" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image12" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Reference:<asp:Image ID="Image15" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelReference" runat="server"></asp:Label>
            </td>
            <td><b>Type:</b></td>
            <td>
                <asp:Label ID="LabelType" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image1" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Bodycorp:<asp:Image ID="Image13" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelBodycorp" runat="server"></asp:Label>
            </td>
             <td><b>Creditor:</b></td>
            <td>
                <asp:Label ID="LabelCreditor" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image14" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Gross :<asp:Image ID="Image17" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelGross" runat="server"></asp:Label>
            </td>
            <td><b>Allocated:<asp:Image ID="Image2" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
            <td>
                <asp:Label ID="LabelAllocated" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                &nbsp;</td>
            <td><b>Account:<asp:Image ID="Image18" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
            <td>
                <asp:Label ID="LabelAccountL" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td colspan="4">
                <img src="Images/dot.gif" height="4px" />
                <div><b>Related Transactions</b></div>
                <div>
                <cc1:jqGridAdv runat="server" ID="jqGridRelated" colNames="['ID','Cinvoice Num','Description','Date', 'DueDate', 'Net','Tax','Gross','Due','Paid']"
                    colModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                        { name: 'CinvoiceNum', index: 'CinvoiceNum', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Description', index: 'Description', width: 200, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Date', index: 'Date', width: 100, align: 'left', search: false},
                        { name: 'DueDate', index: 'DueDate', width: 100, align: 'left', search: false},
                        { name: 'Net', index: 'Net', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Tax', index: 'Tax', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Gross', index: 'Gross', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Due', index: 'Due', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Paid', index: 'Paid', width: 100, editable:true, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]"
                    rowNum="25"
                    rowList="[5, 10, 25, 50, 100]"
                    sortname="CinvoiceNum"
                    sortorder="asc"
                    viewrecords="true"
                    width="700"
                    height="200"
                    url="cpaymentallocate.aspx/jqGridRelatedDataBind"
                    hasID="false"
                    multiselect="false"
                    editurl="cpaymentallocate.aspx/SaveDataFromGrid"
                    inlineNav="true"
                     addVisible="false"
                     afterRowSave="ChangeAllocate"
                    />
                </div>
             </td>
        </tr>
        <tr>
            <td colspan="4" style=" text-align:center;">
                <asp:ImageButton ID="ImageButtonUp" runat="server" PostBackUrl="~/cpaymentallocate.aspx" OnClientClick="return ImageButtonUp_ClientClick()"
                    ImageUrl="~/images/goup.gif" />
                <asp:ImageButton ID="ImageButtonDown" runat="server" PostBackUrl="~/cpaymentallocate.aspx" OnClientClick="return ImageButtonDown_ClientClick()"
                    ImageUrl="~/images/godown.gif" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <img src="Images/dot.gif" height="4px" />
                <div><b>Outstanding Transactions</b></div>
                <cc1:jqGridAdv runat="server" ID="jqGridUnpaid" colNames="['ID','Cinvoice Num','Description','Date', 'DueDate', 'Net','Tax','Gross','Due']"
                    colModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                        { name: 'CinvoiceNum', index: 'CinvoiceNum', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Date', index: 'Date', width: 100, align: 'left', search: false},
                        { name: 'DueDate', index: 'DueDate', width: 100, align: 'left', search: false},
                        { name: 'Net', index: 'Net', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Tax', index: 'Tax', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Gross', index: 'Gross', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Due', index: 'Due', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]"
                    rowNum="25"
                    rowList="[5, 10, 25, 50, 100]"
                    sortname="CinvoiceNum"
                    sortorder="asc"
                    viewrecords="true"
                    width="700"
                    height="200"
                    url="cpaymentallocate.aspx/jqGridUnpaidDataBind"
                    hasID="false"
                    multiselect="true"
                    />
            </td>
        </tr>
    </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title"></div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
