<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="formedit.aspx.cs" Inherits="sapp_sms.formedit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Form Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/formedit.js" type="text/javascript"></script>
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
            <td><b>Form:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelFormID" runat="server" Text="ID"  Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Name(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorCode" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxName"></asp:RequiredFieldValidator>
            </td>
            <td><b>FileName(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxFileName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorNumber" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxFileName"></asp:RequiredFieldValidator>
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
