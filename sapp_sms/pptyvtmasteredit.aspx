<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptyvtmasteredit.aspx.cs" Inherits="sapp_sms.pptyvtmasteredit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Pptymaint Master Edit</title>
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
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            
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
            
            <td><b>Date:</b></td>
            <td>
                <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtenderDate" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxDate"/>  
                   
            </td>
            <td><b>Ref:</b></td>
            <td>
                <asp:TextBox ID="TextBoxRef" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Valuer(*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxValuer" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidator2" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorValuer_ServerValidate" ></asp:CustomValidator>
            </td>
            <td><b>Reinstatement:</b></td>
            <td>
                <asp:TextBox ID="TextBoxReinstatement" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" TargetControlID="TextBoxReinstatement" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Inflation:</b></td>
            <td>
                <asp:TextBox ID="TextBoxInflation" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="TextBoxInflation" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b>Demolition:</b></td>
            <td>
                <asp:TextBox ID="TextBoxDemolition" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" TargetControlID="TextBoxDemolition" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        

        <tr>
            <td><b>Replacement:</b></td>
            <td>
                <asp:TextBox ID="TextBoxReplacement" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" TargetControlID="TextBoxReplacement" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b>Fee:</b></td>
            <td>
                <asp:TextBox ID="TextBoxFee" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" TargetControlID="TextBoxFee" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Gst:</b></td>
            <td>
                <asp:TextBox ID="TextBoxGST" runat="server"></asp:TextBox>
            </td>
            <td></td>
            <td>
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
        <tr> <td colspan="4"></td>
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

