<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bankreconciliation.aspx.cs" Inherits="sapp_sms.bankreconciliation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bank Reconciliation</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/bankreconciliation.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
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
        <table class="details">
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Account (*):</b>
                </td>
                <td valign="top">
                    <div>
                        <ajaxtoolkit:combobox id="ComboBoxAccountCode" runat="server" autopostback="False"
                            dropdownstyle="DropDownList" autocompletemode="SuggestAppend" casesensitive="False"
                            iteminsertlocation="Append" width="150px">
                        </ajaxtoolkit:combobox>
                        <asp:CustomValidator ID="CustomValidatorAccountCode" runat="server" ForeColor="Red"
                            ErrorMessage="!" OnServerValidate="CustomValidatorAccountCode_ServerValidate"></asp:CustomValidator>
                    </div>
                    <div>
                        <asp:Image ID="Image6" runat="server" Height="4px" ImageUrl="~/images/transparent.png" />
                    </div>
                </td>
                <td>
                    <b>Cutoff Date (*):</b>
                </td>
                <td>
                    <div>
                        <asp:TextBox ID="TextBoxCutOffDate" runat="server" ClientIDMode="Static" Height="18px"></asp:TextBox>
                        <ajaxtoolkit:calendarextender runat="server" id="CalendarCutOffDate" format="dd/MM/yyyy"
                            targetcontrolid="TextBoxCutOffDate" cssclass="sappcalendar" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Closing Balance(*):</b>
                </td>
                <td>
                    <asp:TextBox ID="ClosingBalanceT" runat="server" ClientIDMode="Static" Height="18px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorCutOffDate0" runat="server"
                        ErrorMessage="!" ControlToValidate="ClosingBalanceT"></asp:RequiredFieldValidator>
                </td>
                <td>
                    &nbsp;</td>
                <td>
                        &nbsp;</td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Reconciliation</div>
            <div>
                <asp:ImageButton ID="ImageButtonReconcilication" runat="server" ImageUrl="Images/Maintenance.gif"
                    OnClick="ImageButtonReconcilication_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                History</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/Maintenance.gif"
                    CausesValidation="false" OnClick="ImageButton1_Click" />
            </div>
        </div>
        <div class="button" style="display: none;">
            <div class="button-title">
                Report</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="false" ImageUrl="Images/Maintenance.gif"
                    OnClick="ImageButton2_Click" />
            </div>
        </div>
    </div>
</asp:Content>
