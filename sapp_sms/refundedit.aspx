<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="refundedit.aspx.cs" Inherits="sapp_sms.refundedit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<%@ Register src="~/UseControl/BankBalance.ascx" tagname="BankBalance" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Refund Edit</title>
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/receiptedit.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                <asp:Literal ID="LiteralNext" runat="server" Visible="false">Next</asp:Literal></div>
            <div>
                <asp:ImageButton ID="ImageButtonNext" runat="server" ImageUrl="Images/goright.gif"
                    Visible="false" OnClick="ImageButtonNext_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                <asp:Label ID="SaveL" runat="server" Text="Save"></asp:Label></div>
            <div>
                <asp:ImageButton ID="ImageButtonSave" runat="server" ImageUrl="Images/save.gif" OnClick="ImageButtonSave_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Cancel</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false"  OnClick="ImageButtonClose_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>Refund:</b>
                </td>
                <td colspan="3">
                    <b>
                        <asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Ref(*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxRef" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorRef" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxRef"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b>Bank Balance :</b>
                </td>
                <td>
                    <uc1:BankBalance ID="BankBalance1" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Bodycorp:</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ComboBoxBodycorp_SelectedIndexChanged1">
                    </ajaxToolkit:ComboBox>
                </td>
                <td>
                    <b>Unit(*) :</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" onselectedindexchanged="ComboBoxUnit_SelectedIndexChanged">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorDebtor" runat="server" ForeColor="Red" ErrorMessage="!"
                        OnServerValidate="CustomValidatorDebtor_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Debtor(*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxDebtor" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" OnSelectedIndexChanged="ComboBoxDebtor_SelectedIndexChanged">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorUnit" runat="server" ForeColor="Red" ErrorMessage="!"
                        OnServerValidate="CustomValidatorUnit_ServerValidate"></asp:CustomValidator>
                </td>
                <td>
                    <b>Payment Type(*):</b>
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div>
                                <ajaxToolkit:ComboBox ID="ComboBoxPaymentType" runat="server" AutoPostBack="True"
                                    DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" CaseSensitive="False"
                                    ItemInsertLocation="Append" Width="150px" OnSelectedIndexChanged="ComboBoxPaymentType_SelectedIndexChanged">
                                </ajaxToolkit:ComboBox>
                                <asp:CustomValidator ID="CustomValidatorPaymentType" runat="server" ForeColor="Red"
                                    ErrorMessage="!" OnServerValidate="CustomValidatorPaymentType_ServerValidate"></asp:CustomValidator>
                            </div>
                            <div>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Literal ID="LiteralChequeNum" runat="server" Visible="false">Num:</asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBoxChequeNum" runat="server" Visible="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Literal ID="LiteralBank" runat="server" Visible="false">Bank:</asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBoxBank" runat="server" Visible="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Literal ID="LiteralBranch" runat="server" Visible="false">Branch:</asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBoxBranch" runat="server" Visible="false"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Date (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxDate" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxDate"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b>Amount(*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxGross" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorGross" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxGross"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <b>Notes:</b>
                    </td>
            </tr>
            <tr>
                <td colspan="4">
                    <b>
                        <asp:TextBox ID="TextBoxNotes" runat="server" Height="150px" Width="100%" TextMode="MultiLine"></asp:TextBox>
                    </b>
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
