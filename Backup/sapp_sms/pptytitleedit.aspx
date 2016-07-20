<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptytitleedit.aspx.cs" Inherits="sapp_sms.pptytitleedit" %>
<%@ Register TagPrefix="uc" TagName="jqGrid" Src="~/jqGrid.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Pptytitle Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/pptytitledetails.js.js" type="text/javascript"></script>
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
            <td><b>Ctreference:</b></td>
            <td>
                <asp:TextBox ID="TextBoxCtreference" runat="server"></asp:TextBox>
                </td>
            <td><b>Property(*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxProperty" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorProperty" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorProperty_ServerValidate" ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Dplan:</b></td>
            <td>
                <asp:TextBox ID="TextBoxDplan" runat="server"></asp:TextBox>
                   </td>
            <td><b>Lot:</b></td>
            <td>
                <asp:TextBox ID="TextBoxLot" runat="server"></asp:TextBox>
                   </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Area:</b></td>
            <td>
                <asp:TextBox ID="TextBoxArea" runat="server"></asp:TextBox>
                   </td>
            <td><b>Authority(*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxAuthority" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorAuthority" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorAuthority_ServerValidate" ></asp:CustomValidator></td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        

        <tr>
            <td><b>Zone(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxZone" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="TextBoxZone" FilterType="Numbers"></ajaxToolkit:FilteredTextBoxExtender>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxZone"></asp:RequiredFieldValidator>
                   </td>
            <td><b>Note:</b></td>
            <td>
                <asp:TextBox ID="TextBoxNote" runat="server"></asp:TextBox>
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



