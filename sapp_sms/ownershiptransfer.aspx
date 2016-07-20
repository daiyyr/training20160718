<%@ Page Title="" Language="C#" MasterPageFile="~/popup.Master" AutoEventWireup="true"
    CodeBehind="ownershiptransfer.aspx.cs" Inherits="sapp_sms.ownershiptransfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Ownership Transfer</title>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="width: 600px;">
        <table class="popuptb">
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>New Proprietor (*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxDebtor" runat="server" AutoPostBack="True" DropDownStyle="DropDownList" ClientIDMode="Static"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" OnSelectedIndexChanged="ComboBoxDebtor_SelectedIndexChanged">
                    </ajaxToolkit:ComboBox>
                </td>
                <td width="110px">
                    <b>Active Date (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxStart" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderActivityEnd" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxStart" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Panel ID="Panel1" runat="server" Visible="false">
                        <table class="details">
                            <tr>
                                <td>
                                    <b>Debtor:</b><asp:Image ID="Image19" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                                <td colspan="3">
                                    <b>
                                        <asp:Label ID="LabelID0" runat="server" Text="ID" Visible="False"></asp:Label></b>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Image ID="Image20" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <strong>Code:</strong>
                                </td>
                                <td>
                                    <asp:Label ID="LabelCode" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <b>Name:</b>
                                </td>
                                <td>
                                    <asp:Label ID="LabelName" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Image ID="Image21" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Type:</b><asp:Image ID="Image22" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                                <td>
                                    <asp:Label ID="LabelType" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <b>Salutation:</b>
                                </td>
                                <td>
                                    <asp:Label ID="LabelSalutation" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Image ID="Image23" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Payment Term :</b><asp:Image ID="Image24" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                                <td>
                                    <asp:Label ID="LabelPaymentTerm" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <b>Payment Type :</b>
                                </td>
                                <td>
                                    <asp:Label ID="LabelPaymentType" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Image ID="Image14" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Print:</b><asp:Image ID="Image15" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                                <td>
                                    <asp:Label ID="LabelPrint" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <b>Email:</b>&nbsp;
                                </td>
                                <td>
                                    <asp:Label ID="LabelEmail" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Image ID="Image16" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Notes:</b><asp:Image ID="Image17" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                                <td colspan="3">
                                    <asp:Label ID="LabelNotes" runat="server" TextMode="MultiLine" Width="500px"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Image ID="Image18" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="1">
                    <div class="button">
                        <div class="button-title">
                            New Proprietor</div>
                        <div>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/new.gif" OnClick="ImageButton1_Click" />
                        </div>
                    </div>
                    <div class="button">
                        <div class="button-title">
                            Submit</div>
                        <div>
                            <asp:ImageButton ID="ImageButtonSubmit" runat="server" ImageUrl="~/images/submit.gif" PostBackUrl="~/ownershiptransfer.aspx" OnClientClick="return ImageButtonSubmit_ClientClick();" />
                        </div>
                    </div>
                    <div class="button">
                        <div class="button-title">
                            Close</div>
                        <div>
                            <asp:ImageButton ID="ImageButtonClose" runat="server" OnClientClick="window.close(); return false;" ImageUrl="~/images/close.gif" />
                        </div>
                    </div>
                </td>
                <td colspan="3">
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
