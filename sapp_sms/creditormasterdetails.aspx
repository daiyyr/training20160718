<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="creditormasterdetails.aspx.cs" Inherits="sapp_sms.creditormasterdetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - CreditorMaster Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/creditormasterdetails.js" type="text/javascript"></script>
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
                            <b>Creditor:<asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td colspan="3">
                            <b>
                                <asp:Label ID="LabelCreditorID" runat="server" Text="ID" Visible="False"></asp:Label></b>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Code:<asp:Image ID="Image13" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td>
                            <asp:Label ID="LabelCode" runat="server"></asp:Label>
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
                            <asp:Image ID="Image14" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>GST:<asp:Image ID="Image20" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td>
                            <asp:Label ID="LabelGST" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b>Bank AC:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelBankAC" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image21" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td>
                            <b>REF:<asp:Image ID="Image22" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td>
                            <asp:Label ID="Labelref" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b>Payee:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelPayee" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td colspan="4">
                            <asp:Image ID="Image23" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td>
                            <b>Salutation:<asp:Image ID="Image24" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td>
                            <asp:Label ID="LabelSalutation" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b>Payment Type :</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelPaymentType" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td colspan="4">
                            <asp:Image ID="Image25" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Notax:<asp:Image ID="Image26" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td>
                            <asp:Label ID="LabelNotax" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b></b>
                        </td>
                        <td>
                            <asp:Label ID="LabelPaymentTerm" Visible="false" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image27" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Notes:<asp:Image ID="Image28" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td colspan="3">
                            <asp:Label ID="LabelNotes" runat="server" TextMode="MultiLine" Width="500px"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image29" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Contractor Type:<asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                        </td>
                        <td>
                            <asp:Label ID="LabelCntrType" runat="server"></asp:Label>
                        </td>
                        <td>
                            <b>Service Area:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelSrvArea" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabs-2">
                <div>
                    <cc1:jqGridAdv runat="server" ID="jqGridComms" colNames="['ID', 'Type(*)', 'Details(*)', 'Primary', 'Order']"
                        colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                     { name: 'Type', index: 'Type', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'creditormasterdetails.aspx/DataGridCommsTypeSelect'}, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                     { name: 'Details', index: 'Details', width: 200, editable:true,editoptions: { 'maxlength': 100 } ,editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: false},
                     { name: 'Primary', index: 'Primary', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'Yes:No'}, align: 'left', search: false},
                     { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden: true}
                     ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Order" sortorder="asc" viewrecords="true"
                        width="700" height="500" url="creditormasterdetails.aspx/DataGridCommsDataBind"
                        hasID="true" idName="creditorid" inlineNav="true" editurl="creditormasterdetails.aspx/DataGridCommsSave"
                        contentPlaceHolder="ContentPlaceHolder1" multiselect="" />
                </div>
                <div align="left">
                    <asp:Button ID="ButtonDeleteComm" runat="server" Text="Delete" OnClientClick="return ButtonDeleteComm_ClientClick()" />
                </div>
            </div>
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Purchase Order</div>
            <div>
                <asp:ImageButton ID="ImageButtonPurchOrder" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClick="ImageButtonPurchOrder_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Invoice</div>
            <div>
                <asp:ImageButton ID="ImageButtonInvoice" runat="server" ImageUrl="~/images/invoice.gif"
                    OnClick="ImageButtonInvoice_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Payment</div>
            <div>
                <asp:ImageButton ID="ImageButtonPayment" runat="server" ImageUrl="~/images/insurance.gif"
                    OnClick="ImageButtonPayment_Click" />
            </div>
        </div>
    </div>
</asp:Content>
