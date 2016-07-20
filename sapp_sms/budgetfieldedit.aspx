<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="budgetfieldedit.aspx.cs" Inherits="sapp_sms.budgetfieldedit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Budget Field Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/budgetfieldedit.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
    <div class="button">
        <div class="button-title">Save</div>
        <div>
            <asp:ImageButton ID="ImageButtonSave"
                runat="server" ImageUrl="Images/save.gif" onclick="ImageButtonSave_Click"/>
        </div>
    </div>
    <div  class="button">
    <div class="button-title">Cancel</div>
    <div>
        <asp:ImageButton ID="ImageButtonClose"
            runat="server" ImageUrl="Images/close.gif"  CausesValidation="false"  OnClientClick="history.back(); return false;"
            onclick="ImageButtonClose_Click" />
    </div>
    </div>
</div>
    <div id="content_middle">
    <table class="details">
        <tr>
            <td><b>Element ID:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID"  Visible="false"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Name (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorName" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxName"></asp:RequiredFieldValidator>
            </td>
            <td><b>Bodycorp (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Start:</b></td>
            <td>
                <asp:TextBox ID="TextBoxStart" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarStart" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxStart"/>   
            </td>
            <td><b>End:</b></td>
            <td>
                <asp:TextBox ID="TextBoxEnd" runat="server" ></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtenderEnd" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxEnd"/>   
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Order(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxOrder" runat="server" ></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtenderOrder" runat="server" TargetControlID="TextBoxOrder" FilterType="Numbers"></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b><asp:Label ID="LabelRate" Text="Rate" runat="server" ></asp:Label></b></td>
            <td>
                <asp:TextBox ID="TextBoxRate" runat="server" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorRate" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxRate"></asp:RequiredFieldValidator>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxRate" 
                    ID="FilteredTextBoxExtenderRate" runat="server" FilterType="Custom, Numbers" ValidChars="." ></ajaxToolkit:FilteredTextBoxExtender>
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
