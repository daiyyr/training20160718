<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="useredit.aspx.cs" Inherits="sapp_sms.useredit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp Telco - User Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/useredit.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
    <div class="button">
        <div class="button-title">Save</div>
        <div>
            <asp:ImageButton ID="ImageButtonSave"
                runat="server" ImageUrl="Images/save.gif" 
                onclick="ImageButtonSave_Click"   />
        </div>
    </div>
    <div  class="button">
    <div class="button-title">Cancel</div>
    <div>
        <asp:ImageButton ID="ImageButtonClose"
            runat="server" ImageUrl="Images/close.gif" CausesValidation="false"  OnClientClick="history.back();; return false;"
            onclick="ImageButtonClose_Click"  />
    </div>
    </div>
</div>
    <div id="content_middle">
    <table class="details">
        <tr>
            <td><b>User:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelUserID" runat="server" Text="ID" Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Login(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxLogin" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorLogin" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxLogin"></asp:RequiredFieldValidator>
            </td>
            <td><b>Name:</b></td>
            <td>
                <asp:TextBox ID="TextBoxName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Password:</b></td>
            <td>
                <asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password"></asp:TextBox>
            </td>
            <td><b>Privilege:</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxPrivilege" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorPrivilege" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorPrivilege_ServerValidate"  ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Email:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxEmail" runat="server" ></asp:TextBox>
            </td>
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
