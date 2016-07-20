<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="unitmasterdiscount.aspx.cs" Inherits="sapp_sms.unitmasterdiscount" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Master Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/unitmasterdetails.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            width: 180px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" 
                    ImageUrl="~/images/close.gif" OnClick="ImageButtonClose_Click" 
                    OnClientClick="history.back();; return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    Unit:</td>
                <td>
                    <asp:Label ID="UnitNameL" runat="server"></asp:Label>
                </td>
                <td>
                    &nbsp;</td>
                <td class="style1">
                    &nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
                    Invoice
                </td>
                <td>
                    Receipt
                </td>
                <td class="style1">
                    Discount Journal
                </td>
            </tr>
            <tr>
                <td colspan="2" width="33%">
                    <asp:ListBox ID="InvoiceLB" runat="server" AutoPostBack="True" Height="500px" OnSelectedIndexChanged="InvoiceLB_SelectedIndexChanged"
                        Width="100%"></asp:ListBox>
                </td>
                <td width="33%">
                    <asp:ListBox ID="ReceiptLB" runat="server" Height="500px" Width="100%">
                    </asp:ListBox>
                </td>
                <td width="33%">
                    <asp:ListBox ID="DiscountLB" runat="server" Height="500px" Width="100%">
                    </asp:ListBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td class="style1">
                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Link" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
