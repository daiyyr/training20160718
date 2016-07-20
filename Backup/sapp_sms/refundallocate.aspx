<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="refundallocate.aspx.cs" Inherits="sapp_sms.refundallocate" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Refund Allocate</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/refundallocate.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                <asp:Label ID="SaveL" runat="server" Text="Save"></asp:Label></div>
            <div>
                <asp:ImageButton ID="ImageButtonSave" runat="server" ImageUrl="Images/save.gif" OnClick="ImageButtonSave_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Cancel</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false" onclick="ImageButtonClose_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>Element ID:</b>
                </td>
                <td colspan="3">
                    <b>
                        <asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Ref:</b>
                </td>
                <td>
                    <asp:Label ID="LabelRef" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Debtor:</b>
                </td>
                <td>
                    <asp:Label ID="LabelDebtor" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Bodycorp:</b>
                </td>
                <td>
                    <asp:Label ID="LabelBodycorp" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Unit :</b>
                </td>
                <td>
                    <asp:Label ID="LabelUnit" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Payment Type:</b>
                </td>
                <td>
                    <asp:Label ID="LabelPaymentType" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Account :</b>
                </td>
                <td>
                    <asp:Label ID="LabelAccount" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                    <b>Gross :</b>
                </td>
                <td>
                    <asp:Label ID="LabelGross" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Date :</b>
                </td>
                <td>
                    <asp:Label ID="LabelDate" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Allocated :</b>
                </td>
                <td>
                    <asp:Label ID="LabelAllocated" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <b>Note:</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label ID="LabelNote" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <img src="Images/dot.gif" height="4px" />
                    <div>
                        <b>Related Transactions</b></div>
                    <div style="text-align: right;">
                        &nbsp;</div>
                    <div>
                        <cc1:jqGridAdv runat="server" ID="jqGridRelated" colNames="['ID','RefType','Num','Description','Date', 'DueDate', 'Net','Tax','Gross','Due','Paid']"
                            colModel="[
                        { name: 'ID', index: 'ID',editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['cn', 'nc']},hidden:true },
                                                { name: 'RefType', index: 'RefType',editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['cn', 'nc']},hidden:true },

                        { name: 'InvoiceNum', index: 'InvoiceNum', align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Description', index: 'Description',  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Date', index: 'Date',  align: 'left', search: false},
                        { name: 'DueDate', index: 'DueDate', align: 'left', search: false},
                        { name: 'Net', index: 'Net',  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Tax', index: 'Tax', align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Gross', index: 'Gross',align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Due', index: 'Due',  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Paid', index: 'Paid',  editable:true, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="InvoiceNum" sortorder="asc" viewrecords="true"
                            width="800" height="200" url="refundallocate.aspx/jqGridRelatedDataBind" hasID="false"
                            multiselect="false" editurl="refundallocate.aspx/SaveDataFromGrid" inlineNav="true"
                            addVisible="false" afterRowSave="ChangeAllocate" />
                    </div>

                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center;">
                    <asp:ImageButton ID="ImageButtonUp" runat="server" PostBackUrl="~/refundallocate.aspx"
                        OnClientClick="return ImageButtonUp_ClientClick()" ImageUrl="~/images/goup.gif" />
                    <asp:ImageButton ID="ImageButtonDown" runat="server" PostBackUrl="~/refundallocate.aspx"
                        OnClientClick="return ImageButtonDown_ClientClick()" ImageUrl="~/images/godown.gif" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <img src="Images/dot.gif" height="4px" />
                    <div>
                        <b>Outstanding Transactions</b></div>
                    <div>
                        <cc1:jqGridAdv runat="server" ID="jqGridUnpaid" colNames="['ID','RefType','Num','Description','Date', 'DueDate', 'Net','Tax','Gross','Due']"
                            colModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: [ 'cn', 'nc']} , hidden:true},
                                                { name: 'RefType', index: 'RefType',editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['cn', 'nc']},hidden:true },

                        { name: 'InvoiceNum', index: 'InvoiceNum', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Date', index: 'Date', width: 100, align: 'left', search: false},
                        { name: 'DueDate', index: 'DueDate', width: 100, align: 'left', search: false},
                        { name: 'Net', index: 'Net', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Tax', index: 'Tax', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Gross', index: 'Gross', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Due', index: 'Due', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="InvoiceNum" sortorder="asc" viewrecords="true"
                            width="800" height="200" url="refundallocate.aspx/jqGridUnpaidDataBind" hasID="false"
                            multiselect="true" />
                </td>
            </tr>
        </table>
        <script type="text/javascript">
            function JGAjaxErrorFunction() {
                var grid = jQuery("#" + GetClientId("jqGridRelated") + "_datagrid1");
                grid.trigger("reloadGrid");
            }
        </script>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
            </div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
