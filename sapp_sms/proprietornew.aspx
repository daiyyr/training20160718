<%@ Page Title="" Language="C#" MasterPageFile="~/popup.Master" AutoEventWireup="true"
    CodeBehind="proprietornew.aspx.cs" Inherits="sapp_sms.ProprietorNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <base target="_self" />
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/ownershiptransfer.js" type="text/javascript"></script>
    <style type="text/css">
        .button
        {
            border: 1px #369 solid;
        }
        .button-title
        {
            background-color: #369;
            color: #ffffff;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="popuptb">
        <tr>
            <td>
                <b>Debtor:</b>
            </td>
            <td colspan="3">
                <b>
                    <asp:Label ID="LabelCreditorID" runat="server" Text="ID" Visible="False"></asp:Label>
                </b>
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td>
                <b>Name(*):</b>
            </td>
            <td>
                <asp:TextBox ID="TextBoxName" runat="server" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorNumber" runat="server" ErrorMessage="!"
                    ForeColor="Red" ControlToValidate="TextBoxName"></asp:RequiredFieldValidator>
            </td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:TextBox ID="TextBoxCode" runat="server" Visible="False"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <b>Type(*):</b>
            </td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxType" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                    AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                    Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorType" runat="server" ForeColor="Red" ErrorMessage="!"
                    OnServerValidate="CustomValidatorType_ServerValidate"></asp:CustomValidator>
            </td>
            <td>
                <b>Salutation:</b>
            </td>
            <td>
                <asp:TextBox ID="TextBoxSalutation" runat="server" ClientIDMode="Static"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td>
                <b>Payment Term:</b>
            </td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxPaymentTerm" runat="server" AutoPostBack="False"
                    DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" CaseSensitive="False"
                    ItemInsertLocation="Append" Width="150px">
                </ajaxToolkit:ComboBox>
            </td>
            <td>
                <b>Payment Type:</b>
            </td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxPaymentType" runat="server" AutoPostBack="False"
                    DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" CaseSensitive="False"
                    ItemInsertLocation="Append" Width="150px">
                </ajaxToolkit:ComboBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td>
                <b>Print:</b>
            </td>
            <td>
                <asp:CheckBox ID="CheckBoxPrint" runat="server" />
            </td>
            <td>
                <b>Email:</b>
            </td>
            <td>
                <asp:CheckBox ID="CheckBoxEmail" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td>
                <b>Notes:</b>
            </td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" Width="500px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="content_left">
                    <div class="button">
                        <div class="button-title">
                            Save</div>
                        <div>
                            <asp:ImageButton ID="ImageButtonSave" runat="server" ImageUrl="Images/save.gif" OnClick="ImageButtonSave_Click" />
                        </div>
                    </div>
                    <div class="button">
                        <div class="button-title">
                            Cancel</div>
                        <div>
                            <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                                CausesValidation="false" OnClick="ImageButtonClose_Click" />
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
