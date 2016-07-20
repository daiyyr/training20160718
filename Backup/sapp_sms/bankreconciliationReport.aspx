<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bankreconciliationReport.aspx.cs" Inherits="sapp_sms.bankreconciliationReport" %>

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
                        <ajaxToolkit:ComboBox ID="ComboBoxAccountCode0" runat="server" AutoPostBack="False"
                            DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" CaseSensitive="False"
                            ItemInsertLocation="Append" Width="150px">
                        </ajaxToolkit:ComboBox>
                    </div>
                    <div>
                        <asp:Image ID="Image7" runat="server" Height="4px" ImageUrl="~/images/transparent.png" />
                    </div>
                </td>
                <td>
                    <b>Start Date (*):</b>
                </td>
                <td>
                    <div>
                        <asp:Image ID="Image8" runat="server" Height="6px" ImageUrl="~/images/transparent.png" />
                    </div>
                    <div>
                        <asp:TextBox ID="StartDate_T" runat="server" ClientIDMode="Static" Height="18px"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender runat="server" ID="StartDate_T_CalendarExtender" Format="dd/MM/yyyy"
                            TargetControlID="StartDate_T" CssClass="sappcalendar" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorCutOffDate1" runat="server"
                            ErrorMessage="!" ControlToValidate="StartDate_T"></asp:RequiredFieldValidator>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    <b>End Date (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="EndDate_T" runat="server" ClientIDMode="Static" Height="18px"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="EndDate_T_CalendarExtender" Format="dd/MM/yyyy"
                        TargetControlID="EndDate_T" CssClass="sappcalendar" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorCutOffDate2" runat="server"
                        ErrorMessage="!" ControlToValidate="EndDate_T"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Report</div>
            <div>
                <asp:ImageButton ID="ImageButtonReconcilication" runat="server" ImageUrl="Images/Maintenance.gif"
                    OnClick="ImageButton1_Click" />
            </div>
        </div>
        <div class="button">
        </div>
    </div>
</asp:Content>
