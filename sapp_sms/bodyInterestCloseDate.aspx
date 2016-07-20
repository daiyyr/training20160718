<%@ Page Title="" Language="C#" MasterPageFile="~/popup.Master" AutoEventWireup="true"
    CodeBehind="bodyInterestCloseDate.aspx.cs" Inherits="sapp_sms.bodyInterestCloseDate" %>

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
    <div style="width: 600px;">
        <asp:Panel ID="PanelStart" runat="server">
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td colspan="4">
                        <b>Interest Close date</b>:<asp:TextBox ID="Date_T" runat="server" ClientIDMode="Static"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" CssClass="sappcalendar"
                            Format="dd/MM/yyyy" TargetControlID="Date_T" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="ButtonInsert" runat="server" Text="Submit" OnClick="ButtonInsert_Click" />
                        <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
