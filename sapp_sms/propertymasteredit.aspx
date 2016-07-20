<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="propertymasteredit.aspx.cs" Inherits="sapp_sms.propertymasteredit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Property Master Edit</title>
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
                <asp:CustomValidator ID="CustomValidatorBodycorp" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorBodycorp_ServerValidate" ></asp:CustomValidator>
            </td>
            <td><b>Type:</b></td>
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
            <td><b>Total SQM:</b></td>
            <td>
                <asp:TextBox ID="TextBoxTotalSqm" runat="server" ReadOnly="True" BackColor="Control"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxTotalSqm" 
                    ID="FilteredTextBoxExtender3" runat="server" FilterType="Numbers" ></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b>Num Of Units:</b></td>
            <td>
                <asp:TextBox ID="TextBoxNumOfUnits" runat="server" ReadOnly="True" BackColor="Control"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Begin Date (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxBeginDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtender3" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxBeginDate"/>   
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxBeginDate"></asp:RequiredFieldValidator>
            </td>
            <td><b></b></td>
            <td>
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
