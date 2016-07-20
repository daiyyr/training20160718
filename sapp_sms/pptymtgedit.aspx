<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptymtgedit.aspx.cs" Inherits="sapp_sms.pptymtgedit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Pptymtg Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
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
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID"  Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Mortgagor (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxMortgagor" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorCode" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxMortgagor"></asp:RequiredFieldValidator>
            </td>
            <td><b>Property (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxProperty" runat="server"
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
            <td><b>Date:</b></td>
            <td>
                <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarDate" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxDate"/>     
            </td>
            <td><b>Principle:</b></td>
            <td>
                <asp:TextBox ID="TextBoxPrinciple" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" TargetControlID="TextBoxPrinciple" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Rate:</b></td>
            <td>
                <asp:TextBox ID="TextBoxRate" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="TextBoxRate" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b>Payment:</b></td>
            <td>
                <asp:TextBox ID="TextBoxPayment" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" TargetControlID="TextBoxPayment" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
                </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        

        <tr>
            <td><b>Expiry:</b></td>
            <td>
                <asp:TextBox ID="TextBoxExpiry" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtenderExpiry" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxExpiry"/>     
                <asp:HiddenField ID="OldEndHF" runat="server" />
            </td>
            <td><b>Term:</b></td>
            <td>
                <asp:TextBox ID="TextBoxTerm" runat="server" /></td>
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


