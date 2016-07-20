<%@ Page Title="" Language="C#" MasterPageFile="~/popup.Master" AutoEventWireup="true"
    CodeBehind="cinvoiceCpayment.aspx.cs" Inherits="sapp_sms.cinvoiceCpayment" %>

<%@ Register src="~/UseControl/BankBalance.ascx" tagname="BankBalance" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bank Reconciliation Insert</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/bankreconcileinsert.js" type="text/javascript"></script>
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="width: 600px;" class="details">
        <asp:Panel ID="PanelStart" runat="server">
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td>
                        Bank Balance:&nbsp;&nbsp;
                        <uc1:BankBalance ID="BankBalance2" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Type:
                        <ajaxToolkit:ComboBox ID="TypeDL" runat="server" 
                            AutoCompleteMode="SuggestAppend" AutoPostBack="False" CaseSensitive="False" 
                            DropDownStyle="DropDownList" ItemInsertLocation="Append" Width="150px">
                        </ajaxToolkit:ComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Reference:<asp:TextBox ID="ReferenceT" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Amount:<asp:TextBox ID="AmountT" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Date:<asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender ID="CalendarDate" runat="server" 
                            CssClass="sappcalendar" Format="dd/MM/yyyy" TargetControlID="TextBoxDate" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="ButtonInsert" runat="server" Text="Submit" OnClick="ButtonInsert_Click" />
                        <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
