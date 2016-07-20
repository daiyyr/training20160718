<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="creditormasteredit.aspx.cs" Inherits="sapp_sms.creditormasteredit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Creditor Master Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/jquery.ui.timepicker.css" rel="Stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
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
            <td><b>Creditor:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelCreditorID" runat="server" Text="ID" Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Code(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxCode" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorCode" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxCode"></asp:RequiredFieldValidator>
            </td>
            <td><b>Name(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorNumber" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxName"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>GST:</b></td>
            <td>
                <asp:TextBox ID="TextBoxGST" runat="server"></asp:TextBox>
            </td>
            <td><b>Bank AC:</b></td>
            <td>
                <asp:TextBox ID="TextBoxBankAC" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr style="display:none;">
            <td><b>REF:</b></td>
            <td>
                <asp:TextBox ID="TextBoxref" runat="server"></asp:TextBox>
            </td>
            <td><b>Payee:</b></td>
            <td>
                <asp:TextBox ID="TextBoxPayee" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr style="display:none;">
            <td colspan="4"></td>
        </tr>
        <tr style="display:none;">
            <td><b>Salutation:</b></td>
            <td>
                <asp:TextBox ID="TextBoxSalutation" runat="server"></asp:TextBox>
            </td>
            <td><b>Payment Type:</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxPaymentType" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
            </td>
        </tr>
        <tr style="display:none;">
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Notax:</b></td>
            <td>
                <asp:CheckBox ID="CheckBoxNotax" runat="server" />
            </td>
            <td>&nbsp;</td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxPaymentTerm" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px" Visible="False">
                </ajaxToolkit:ComboBox>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Notes:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" Width="500px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
         <tr>
            <td><b>Contractor Type:</b></td>
            <td>
                <asp:TextBox ID="TextBoxCntrType" runat="server"></asp:TextBox>
            </td>
            <td><b>Service Area:</b></td>
            <td>
                <asp:TextBox ID="TextBoxSrvArea" runat="server"></asp:TextBox>
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
