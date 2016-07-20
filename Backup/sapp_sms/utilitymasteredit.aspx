<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="utilitymasteredit.aspx.cs" Inherits="sapp_sms.utilitymasteredit" %>
<%@ Register TagPrefix="uc" TagName="jqGrid" Src="~/jqGrid.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Master Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
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
                runat="server" ImageUrl="Images/close.gif"  CausesValidation="false"  OnClientClick="history.back();; return false;"
                onclick="ImageButtonClose_Click" />
        </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
        <tr>
            <td><b>Element ID:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Unit (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorType0" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorUnit_ServerValidate" ></asp:CustomValidator>
                </td>
            <td><b>Type (*):</b></td>
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
            <td><b>Reading (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxReading" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="TextBoxReading" FilterType="Numbers">
                </ajaxToolkit:FilteredTextBoxExtender>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxReading"></asp:RequiredFieldValidator>
                   </td>
            <td><b>Date (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtenderDate" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxDate"/>  
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxDate"></asp:RequiredFieldValidator>   </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Batch (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxBatch" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                    ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxBatch"></asp:RequiredFieldValidator>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" TargetControlID="TextBoxBatch" FilterType="Numbers">
                </ajaxToolkit:FilteredTextBoxExtender>
                   </td>
            <td><b>Unit Price (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxUnitPrice" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" TargetControlID="TextBoxUnitPrice" FilterType="Custom, Numbers" ValidChars=".">
                    </ajaxToolkit:FilteredTextBoxExtender>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                    ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxUnitPrice"></asp:RequiredFieldValidator>
                   </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        

        <tr>
            <td><b>Notes:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" 
                    Width="500px"></asp:TextBox>
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



