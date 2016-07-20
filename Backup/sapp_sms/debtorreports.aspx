<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="debtorreports.aspx.cs" Inherits="sapp_sms.debtorreports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Debtor Report Selection</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/debtorreports.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server"   OnClientClick="window.close(); return false;"
                    ImageUrl="~/images/close.gif"/>
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <asp:CheckBox ID="CheckBoxProprietorList" runat="server" />
                    <b>Proprietor List Report</b>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="border-width: 0px; border-spacing: 0px; " cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan="4" style="border-width: 0px">
                                <asp:CheckBox ID="CheckBoxProprietorStatement" runat="server" />
                                <b>Proprietor Statement Report</b>
                                <asp:DropDownList ID="DropDownProprietorStatement" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="border-width: 0px; text-align: right;">
                                <b>Period(*):<asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                            <td style="border-width: 0px">
                                <asp:TextBox ID="TextBoxDateStart" runat="server"></asp:TextBox>
                                <ajaxtoolkit:calendarextender runat="server" id="CalendarDate" cssclass="sappcalendar" format="dd/MM/yyyy" targetcontrolid="TextBoxDateStart"></ajaxtoolkit:calendarextender>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!" ForeColor="Red" ControlToValidate="TextBoxDateStart"></asp:RequiredFieldValidator>
                            </td>
                            <td style="border-width: 0px">
                                <b>To(*)</b>
                            </td>
                            <td style="border-width: 0px">
                                <asp:TextBox ID="TextBoxDateEnd" runat="server"></asp:TextBox>
                                <ajaxtoolkit:calendarextender runat="server" id="CalendarExtender1" cssclass="sappcalendar" format="dd/MM/yyyy" targetcontrolid="TextBoxDateEnd"></ajaxtoolkit:calendarextender>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="!" ForeColor="Red" ControlToValidate="TextBoxDateEnd"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

        </table>   
    </div>
    <div id="content_right">
        <div class="button">
        <div class="button-title">OK</div>
        <div>
            <asp:ImageButton ID="ImageButtonSubmit" ImageUrl="~/images/submit.gif" 
                runat="server" onclick="ImageButtonSubmit_Click"/>
        </div>
        </div>
    </div>
</asp:Content>
