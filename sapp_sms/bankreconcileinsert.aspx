<%@ Page Title="" Language="C#" MasterPageFile="~/popup.Master" AutoEventWireup="true"
    CodeBehind="bankreconcileinsert.aspx.cs" Inherits="sapp_sms.bankreconcileinsert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bank Reconciliation Insert</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />

    <base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="width: 600px;">
        <asp:Panel ID="PanelStart" runat="server">
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:RadioButtonList ID="RadioButtonListInsert" runat="server">
                            <asp:ListItem Selected="true">Receipt</asp:ListItem>
                            <asp:ListItem>Payment</asp:ListItem>
                            <asp:ListItem>Cash Deposit</asp:ListItem>
                            <asp:ListItem>Cash Payment</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="ButtonInsert" runat="server" Text="Insert" OnClick="ButtonInsert_Click" />
                        <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="PanelReceipt" runat="server" Visible="false">
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td>
                        <b>Receipt:</b>
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorRef" runat="server" ControlToValidate="TextBoxRef"
                            ErrorMessage="!" ForeColor="Red"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <b>Unit(*) :</b>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server" AutoCompleteMode="Suggest"
                            AutoPostBack="true" DropDownStyle="DropDownList" ItemInsertLocation="Append"
                            OnSelectedIndexChanged="ComboBoxUnit_SelectedIndexChanged" Width="150px">
                        </ajaxToolkit:ComboBox>
                        <asp:CustomValidator ID="CustomValidatorDebtor" runat="server" ErrorMessage="!" ForeColor="Red"
                            OnServerValidate="CustomValidatorDebtor_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;<b>Debtor(*):</b>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ComboBoxDebtor" runat="server" AutoPostBack="true" DropDownStyle="DropDownList"
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
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Date (*):</b>
                    </td>
                    <td>
                        <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate" Format="dd/MM/yyyy"
                            CssClass="sappcalendar" TargetControlID="TextBoxDate" />
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
                    <td>
                        <strong>Note:</strong>
                    </td>
                    <td colspan="3">
                        <b>
                            <asp:TextBox ID="TextBoxNotes" runat="server" Height="50px" TextMode="MultiLine"
                                Width="100%"></asp:TextBox>
                        </b>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="ButtonReceipt" runat="server" Text="Submit" OnClick="ButtonReceipt_Click" />
                        <asp:Button ID="ButtonReceiptClose" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="PanelPayment" runat="server" Visible="false">
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td>
                        <b>Cpayment:</b>
                    </td>
                    <td colspan="3">
                        <b>
                            <asp:Label ID="Label1" runat="server" Text="ID" Visible="false"></asp:Label></b>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Bodycorp:</b>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                    </td>
                    <td>
                        <b>Creditor(*):</b>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ComboBoxCreditor" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px" OnSelectedIndexChanged="ComboBoxCreditor_SelectedIndexChanged">
                        </ajaxToolkit:ComboBox>
                        <asp:CustomValidator ID="CustomValidatorCreditor" runat="server" ForeColor="Red"
                            ErrorMessage="!" OnServerValidate="CustomValidatorCreditor_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Reference:</b>
                    </td>
                    <td>
                        <asp:TextBox ID="TextBoxReference" runat="server"></asp:TextBox>
                    </td>
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
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Gross (*):</b>
                    </td>
                    <td>
                        <asp:TextBox ID="TextBoxPymtGross" runat="server"></asp:TextBox>
                        <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxPymtGross" ID="FilteredTextBoxExtender3"
                            runat="server" FilterType="Custom, Numbers" ValidChars=".">
                        </ajaxToolkit:FilteredTextBoxExtender>
                        <asp:Label ID="LabelGross" runat="server" ClientIDMode="Static" ForeColor="#006600"></asp:Label>
                    </td>
                    <td>
                        <b>Date (*):</b>
                    </td>
                    <td>
                        <asp:TextBox ID="TextBoxPymtDate" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender1" Format="dd/MM/yyyy"
                            CssClass="sappcalendar" TargetControlID="TextBoxPymtDate" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!"
                            ForeColor="Red" ControlToValidate="TextBoxPymtDate"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="ButtonPayment" runat="server" Text="Submit" OnClick="ButtonPayment_Click" />
                        <asp:Button ID="ButtonPaymentClose" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%">
                            <Columns>
                                <asp:BoundField DataField="cinvoice_num" HeaderText="Num" />
                                <asp:BoundField DataField="cinvoice_date" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="cinvoice_due" HeaderText="Due" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="cinvoice_description" HeaderText="Description" />
                                <asp:BoundField DataField="cinvoice_gross" HeaderText="Gross" />
                                <asp:BoundField DataField="cinvoice_paid" HeaderText="Paid" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="PanelCashDeposit" runat="server" Visible="false">
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td>
                        <b>Cash Deposit:</b>
                    </td>
                    <td colspan="3">
                        <b>
                            <asp:Label ID="CashDeposit" runat="server" Text="ID" Visible="False"></asp:Label></b>
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
                        <asp:TextBox ID="TextBoxCashDepositRef" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <b>Chart(*):</b>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ChartCombobox" runat="server" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Bodycorp:</b>&nbsp;
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ComboBoxCashDepositBodycorp" runat="server" AutoCompleteMode="SuggestAppend"
                            AutoPostBack="False" CaseSensitive="False" DropDownStyle="DropDownList" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
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
                        <asp:TextBox ID="TextBoxCashDepositDate" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate0" Format="dd/MM/yyyy"
                            CssClass="sappcalendar" TargetControlID="TextBoxCashDepositDate" />
                    </td>
                    <td>
                        <b>Amount(*):</b>
                    </td>
                    <td>
                        <asp:TextBox ID="TextBoxCashDepositGross" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="ButtonCashDeposit" runat="server" Text="Submit" OnClick="ButtonCashDeposit_Click" />
                        <asp:Button ID="ButtonButtonCashDepositClose" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="PanelCashCashPayment" runat="server" Visible="false">
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td>
                        <b>Cash Payment:</b>
                    </td>
                    <td colspan="3">
                        <b>
                            <asp:Label ID="CashPayment" runat="server" Text="ID" Visible="False"></asp:Label></b>
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
                        <asp:TextBox ID="TextBoxCashPaymentRef" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <b>Chart(*):</b>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ChartCashPaymentCombobox" runat="server" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Bodycorp:</b>&nbsp;
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ComboBoxCashPaymentBodycorp" runat="server" AutoCompleteMode="SuggestAppend"
                            AutoPostBack="False" CaseSensitive="False" DropDownStyle="DropDownList" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
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
                        <asp:TextBox ID="TextBoxCashPaymentDate" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate1" Format="dd/MM/yyyy"
                            CssClass="sappcalendar" TargetControlID="TextBoxCashPaymentDate" />
                    </td>
                    <td>
                        <b>Amount(*):</b>
                    </td>
                    <td>
                        <asp:TextBox ID="TextBoxCashPaymentGross" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="ButtonCashPayment" runat="server" Text="Submit" OnClick="ButtonCashPayment_Click" />
                        <asp:Button ID="ButtonButtonCashDepositClose0" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
