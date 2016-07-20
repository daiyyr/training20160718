<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="financialreports.aspx.cs" Inherits="sapp_sms.financialreports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Financial Report Selection</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/financialreports.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonHome" runat="server" OnClientClick="window.close(); return false;"
                    ImageUrl="~/images/close.gif" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>Period(*):<asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxDateStart" runat="server">01/12/2012</asp:TextBox>
                    <ajaxtoolkit:calendarextender runat="server" id="CalendarDate" cssclass="sappcalendar"
                        format="dd/MM/yyyy" targetcontrolid="TextBoxDateStart">
                    </ajaxtoolkit:calendarextender>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxDateStart"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b>To(*)</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxDateEnd" runat="server"></asp:TextBox>
                    <ajaxtoolkit:calendarextender runat="server" id="CalendarExtender1" cssclass="sappcalendar"
                        format="dd/MM/yyyy" targetcontrolid="TextBoxDateEnd">
                    </ajaxtoolkit:calendarextender>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxDateEnd"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Year Begins(*):<asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TextBoxYearBegins" runat="server">01/12/2012</asp:TextBox>
                    <ajaxtoolkit:calendarextender runat="server" id="CalendarExtender2" cssclass="sappcalendar"
                        format="dd/MM/yyyy" targetcontrolid="TextBoxYearBegins">
                    </ajaxtoolkit:calendarextender>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxYearBegins"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxProfitLoss" runat="server" />
                    <b>Profit and Loss</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxBalanceSheet" runat="server" />
                    <b>Balance Sheet</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxTrialBalance" runat="server" />
                    <b>Trial Balance</b>&nbsp;
                    <asp:DropDownList ID="TB_DL" runat="server">
                        <asp:ListItem Value="0">Used Only</asp:ListItem>
                        <asp:ListItem Value="1">All</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxActivity" runat="server" />
                    <b>Account Activity</b>
                    <asp:DropDownList ID="ACChartDL" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxCashPosition" runat="server" />
                    <b>Cash Position</b>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="display: none">
                    <asp:CheckBox ID="CheckBoxCashPositionDetailed" runat="server" Enabled="false" />
                    <b>Cash Position Detailed</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxAgedProprietors" runat="server" />
                    <b>Aged Proprietors</b>
                    <asp:DropDownList ID="AgePDL" runat="server">
                        <asp:ListItem Value="1">Due Base</asp:ListItem>
                        <asp:ListItem Value="2">Apply Base</asp:ListItem>
                        <asp:ListItem Value="3">Invoice Base</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="AgedProprietorPIDDL" runat="server">
                        <asp:ListItem>ALL</asp:ListItem>
                    </asp:DropDownList>
                    <asp:CheckBox ID="CheckBoxShowName" ClientIDMode="Static" runat="server" Text="Show Proprietors Name" Checked="true"/>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxAgedCreditors" runat="server" />
                    <b>Aged Creditors</b>
                    <asp:DropDownList ID="AgeCDL" runat="server">
                        <asp:ListItem Value="1">Due Base</asp:ListItem>
                        <asp:ListItem Value="2">Invoice Base</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxCreditorActivity" runat="server" />
                    <b>Creditor Activity</b>
                </td>
            </tr>
            <tr>
                <td colspan="4" >
                    <asp:CheckBox ID="CheckBoxGSTRe0" runat="server" />
                    <b>GST Reconciliation</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxGST" runat="server" />
                    <b>GST Report Invoice Base Detail</b>
                </td>
            </tr>
            <tr>
            <td colspan="4" style="display: none">
                    <asp:CheckBox ID="CheckBoxGSTRe" runat="server" />
                    <b>GST Report Cash Base </b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxGSTReDetail" runat="server" />
                    <b>GST Report Cash Base Detail</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="JournalReportCK" runat="server" />
                    <b>Journal Report</b>
                    <asp:TextBox ID="JournalT" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="CheckBoxAgedPDetail" runat="server" />
                    <b>Proprietor Detailed Aged</b>
                    <asp:DropDownList ID="AgedProprietorPIDDL0" runat="server">
                        <asp:ListItem>ALL</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="AgedProprietorOutstanding" runat="server">
                        <asp:ListItem Value="0">All</asp:ListItem>
                        <asp:ListItem Value="1">Outstanding</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:CheckBox ID="BMPF_CK" runat="server" />
                    <b>Statement of Equity Movement</b>
                    <asp:DropDownList ID="SM_DL" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                OK</div>
            <div>
                <asp:ImageButton ID="ImageButtonSubmit" ImageUrl="~/images/submit.gif" runat="server"
                    OnClick="ImageButtonSubmit_Click" />
            </div>
        </div>
    </div>
</asp:Content>
