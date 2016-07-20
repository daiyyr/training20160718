<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptycntrmasteredit.aspx.cs" Inherits="sapp_sms.pptycntrmasteredit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Pptycntr Master Edit</title>
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
            
            <td><b>Service (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxService" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorService" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorService_ServerValidate" ></asp:CustomValidator>
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
                <asp:CustomValidator ID="CustomValidator1" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorProperty_ServerValidate" ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            
            <td><b>Creditor (*):</b></td>
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
            <td><b>Expiry:</b></td>
            <td>
                <asp:TextBox ID="TextBoxExpiry" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtenderExpiry" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxExpiry"/>   
                <asp:HiddenField ID="OldEndHF" runat="server" />
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Inactive (*):</b></td>
            <td>
                <asp:CheckBox ID="CheckBoxInactive" runat="server" />
            </td>
            <td><b></b></td>
            <td>
            </td>
        </tr>
        

        <tr>
            <td><b>Notes:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" 
                    Width="500px" />
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


