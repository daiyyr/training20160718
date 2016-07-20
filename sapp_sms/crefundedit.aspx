<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="crefundedit.aspx.cs" Inherits="sapp_sms.crefundedit" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - CRefund Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
        <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/cpaymentedit.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title"> <asp:Label ID="SaveL" runat="server" Text="Save"></asp:Label></div>
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
            <td><b>CRefund:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="false"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Bodycorp:</b></td>
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
             <td><b>Creditor(*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxCreditor" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorCreditor" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorCreditor_ServerValidate" ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>


        <tr>
            <td><b>Reference:</b></td>
            <td>
                <asp:TextBox ID="TextBoxReference" runat="server"></asp:TextBox>
            </td>
            <td><b>Type(*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxType" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorType" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorType_ServerValidate" ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Gross (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxGross" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxGross" 
                    ID="FilteredTextBoxExtender3" runat="server" FilterType="Custom, Numbers" ValidChars="." ></ajaxToolkit:FilteredTextBoxExtender>
                <asp:Label ID="LabelGross" runat="server" ClientIDMode="Static" 
                    ForeColor="#006600" Visible="False"></asp:Label>
            </td>
            <td><b>Date (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarDate" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxDate"/>   
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxDate"></asp:RequiredFieldValidator>       
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




