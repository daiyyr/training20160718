<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="companyedit.aspx.cs" Inherits="sapp_sms.companyedit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <title>Sapp SMS - Company Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/jquery.ui.timepicker.css" rel="Stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/common.js" type="text/javascript"></script>
    <script src="Scripts/accountedit.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div id="content_left">
    <div class="button">
        <div class="button-title">Save</div>
        <div>
            <asp:ImageButton ID="ImageButtonSave"
                runat="server" ImageUrl="Images/save.gif" 
                onclick="ImageButtonSave_Click"  />
        </div>
    </div>
    <div  class="button">
    <div class="button-title">Cancel</div>
    <div>
        <asp:ImageButton ID="ImageButtonClose"
            runat="server" ImageUrl="Images/close.gif" CausesValidation="false"  OnClientClick="history.back(); return false;"
            onclick="ImageButtonClose_Click" />
    </div>
    </div>
    </div>
    <div id="content_middle">
    <table class="details">
        <tr>
            <td><b>Company:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelCompanyID"  Visible="false" runat="server" Text="ID"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Name(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxName" runat="server" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorNumber" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxName"></asp:RequiredFieldValidator>
            </td>
            <td><b>GST:</b></td>
            <td>
                <asp:TextBox ID="TextBoxGST" ClientIDMode="Static" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Bank Num:</b></td>
            <td>
                <asp:TextBox ID="TextBoxBankNum" runat="server" ClientIDMode="Static"></asp:TextBox>
            </td>
            <td><b>Financial Year Begin:</b></td>
            <td>
                <asp:TextBox ID="TextBoxFinancialYrBegin" runat="server" ClientIDMode="Static"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server" CssClass="sappcalendar"
                    ID="CalendarExtender2"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxFinancialYrBegin"/>   
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
    </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title"></div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
