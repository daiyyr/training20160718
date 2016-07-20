<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="accountedit.aspx.cs" Inherits="sapp_sms.accountedit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Account Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/accountedit.js" type="text/javascript"></script>
    <script type="text/javascript">
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>Account:</b>
                </td>
                <td colspan="3">
                    <b>
                        <asp:Label ID="LabelAccountID" runat="server" Text="ID" Visible="false"></asp:Label></b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Code(*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxCode" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorCode" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxCode"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b>Chart Num (*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxChart" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorChart" runat="server" ForeColor="Red" ErrorMessage="!"
                        OnServerValidate="CustomValidatorChart_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <td>
                <b>Name(*):</b>
            </td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxName" runat="server" Width="500px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorNumber" runat="server" ErrorMessage="!"
                    ForeColor="Red" ControlToValidate="TextBoxName"></asp:RequiredFieldValidator>
            </td>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Account Number:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxNum" runat="server"></asp:TextBox>
                </td>
                <td>
                    <b>Bank:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxBank" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Branch:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxBranch" runat="server"></asp:TextBox>
                </td>
                <td>
                    <b>Swift Code:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxSwift" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Description:</b>
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TextBoxDescription" runat="server" TextMode="MultiLine" Width="500px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
            </div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
