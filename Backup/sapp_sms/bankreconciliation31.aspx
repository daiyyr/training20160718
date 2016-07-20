<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bankreconciliation31.aspx.cs" Inherits="sapp_sms.bankreconciliation31" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bank Reconciliation</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/bankreconciliation.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false" OnClientClick="history.back(); return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>Account (*):</b>
                </td>
                <td valign="top">
                    <div>
                        <ajaxToolkit:ComboBox ID="ComboBoxAccountCode" runat="server" AutoPostBack="True"
                            DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" CaseSensitive="False"
                            ItemInsertLocation="Append" Width="150px" OnSelectedIndexChanged="ComboBoxAccountCode_SelectedIndexChanged">
                        </ajaxToolkit:ComboBox>
                    </div>
                    <div>
                        <asp:Image ID="Image6" runat="server" Height="4px" ImageUrl="~/images/transparent.png" />
                    </div>
                    <div>
                        <asp:Image ID="Image1" runat="server" Height="6px" ImageUrl="~/images/transparent.png" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td valign="top">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                        Width="100%" onselectedindexchanged="GridView1_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="false" CommandName=""
                                        Text="Select" BID='<%# Eval("Batch") %>' End='<%# Eval("Cutoff") %>'
                                        OnClick="LinkButton1_Click"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Batch">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Batch") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Batch") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Cutoff" HeaderText="Cutoff Date" DataFormatString="{0:dd/MM/yyyy}" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                <asp:Literal ID="LiteralPDF" Visible="false" runat="server">Download Report</asp:Literal></div>
            <div>
                <asp:ImageButton ID="ImageButtonPDF" Visible="false" runat="server" 
                    ImageUrl="Images/Maintenance.gif" onclick="ImageButtonPDF_Click"  />
            </div>
        </div>
    </div>
</asp:Content>
