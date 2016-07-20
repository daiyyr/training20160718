<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="debtormasterdetails.aspx.cs" Inherits="sapp_sms.debtormasterdetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - DebtorMaster Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/debtormasterdetails.js" type="text/javascript"></script>
    <style type="text/css">
        .WrappedLabel
        {
            width: 100px;
            overflow: hidden;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="~/images/edit.gif"
                    OnClick="ImageButtonEdit_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                    OnClick="ImageButtonDelete_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" OnClientClick="history.back(); return false;"
                    ImageUrl="~/images/close.gif" OnClick="ImageButtonClose_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">General</a></li>
                <li><a href="#tabs-2">Comms</a></li>
            </ul>
            <div id="tabs-1">
                <table class="details">
                    <tr>
                        <td>
                            <b>Debtor:</b><asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td colspan="3">
                            <b>
                                <asp:Label ID="LabelID" runat="server" Text="ID" Visible="False"></asp:Label></b>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong>Code:</strong>
                        </td>
                        <td>
                            <asp:Label ID="LabelCode" runat="server" CssClass="WrappedLabel"></asp:Label>
                        </td>
                        <td>
                            <b>Name:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Type:</b><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td>
                            <asp:Label ID="LabelType" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b>Salutation:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelSalutation" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Payment Term :</b><asp:Image ID="Image13" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td>
                            <asp:Label ID="LabelPaymentTerm" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b>Payment Type :</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelPaymentType" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <strong>COMM BY:</strong>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Print:</b><asp:Image ID="Image15" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td>
                            <asp:Label ID="LabelPrint" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b>Email:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelEmail" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image16" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Notes:</b><asp:Image ID="Image17" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td colspan="3">
                            <asp:Label ID="LabelNotes" runat="server" TextMode="MultiLine" Width="500px"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image18" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabs-2">
                <div>
                    <cc1:jqGridAdv runat="server" ID="jqGridComms" colNames="['ID', 'Type(*)', 'Details(*)', 'Primary', 'Order']"
                        colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                     { name: 'Type', index: 'Type', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'debtormasterdetails.aspx/DataGridCommsTypeSelect'}, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                     { name: 'Details', index: 'Details', width: 200, editable:true,editoptions: { 'maxlength': 100 } ,editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: false},
                     { name: 'Primary', index: 'Primary', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'Yes:No'}, align: 'left', search: false},
                     { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden: true}
                     ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Order" sortorder="asc" viewrecords="true"
                        width="700" height="500" url="debtormasterdetails.aspx/DataGridCommsDataBind"
                        hasID="true" idName="debtorid" inlineNav="true" editurl="debtormasterdetails.aspx/DataGridCommsSave"
                        contentPlaceHolder="ContentPlaceHolder1" multiselect="" />
                </div>
                <div align="left">
                    <asp:Button ID="ButtonDeleteComm" runat="server" Text="Delete" OnClientClick="return ButtonDeleteComm_ClientClick()" /></div>
            </div>
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Invoice</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/invoice.gif"
                    OnClick="ImageButtonInvoice_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Receipt</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/insurance.gif"
                    OnClick="ImageButtonReceipt_Click" />
                <div class="button-title">
                    Refund</div>
                <div>
                    <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/insurance.gif"
                        OnClick="ImageButton3_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
