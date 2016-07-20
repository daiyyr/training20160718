<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bankreconciliation2.aspx.cs" Inherits="sapp_sms.bankreconciliation2" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bank Reconciliation Page 2</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/bankreconciliation2.js" type="text/javascript"></script>
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
                    CausesValidation="false" OnClientClick="history.back(); return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <div>
                        <b>Reonciled Transactions</b></div>
                    <div style="text-align: right;">
                        Opening Balance:<asp:Label ID="OpenBalanceL" runat="server"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <img src="Images/dot.gif" height="4px" />
                    <div>
                        <cc1:jqGridAdv runat="server" ID="jqGridReonciled" colNames="['ID','Date','Ref','Description','Deposit', 'Payment']"
                            colModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                         { name: 'Date', index: 'Date', width: 150, align: 'left', search: true, formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}},
                           
                             { name: 'Ref', index: 'Ref', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Deposit', index: 'Deposit', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Payment', index: 'Payment', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rowNum="10000" rowList="[]" sortname="Date" sortorder="asc" viewrecords="true" width="700"
                            height="300" url="bankreconciliation2.aspx/jqGridReonciledDataBind" hasID="true"
                            idName="accountid" multiselect="true" footerrow="true" userDataOnFooter="true" />
                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">
                    <table width="300" style="float: right">
                        <tr>
                            <td>
                                Closing On Statement:
                            </td>
                            <td>
                                <asp:Label ID="CUBL" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;Closing Balance
                            </td>
                            <td>
                                <asp:Label ID="CBL" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:ImageButton ID="ImageButtonUp" runat="server" OnClientClick="return ImageButtonUp_ClientClick()"
                        ImageUrl="~/images/goup.gif" />
                    <asp:ImageButton ID="ImageButtonDown" runat="server" OnClientClick="return ImageButtonDown_ClientClick()"
                        ImageUrl="~/images/godown.gif" />
                </td>
            </tr>
            <tr>
                <td>
                    <img src="Images/dot.gif" height="4px" />
                    <div>
                        <b>Unreconciled Transactions<asp:HiddenField ID="account_id_HF" runat="server" />
                        </b>
                        <asp:HiddenField ID="cutoffdate_HF" runat="server" />
                    </div>
                    <cc1:jqGridAdv runat="server" ID="jqGridUnreconciled" colNames="['ID','Date','CD','Ref','Description','Deposit', 'Payment']"
                        colModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
               
           { name: 'Date', index: 'Date', width: 150, align: 'left', search: true, formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}},
            { name: 'CD', index: 'CD', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
        { name: 'Ref', index: 'Ref', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  
                         { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Deposit', index: 'Deposit', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Payment', index: 'Payment', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rowNum="10000" rowList="[]" sortname="Date" sortorder="asc" viewrecords="true"
                        width="700" height="300" url="bankreconciliation2.aspx/jqGridUnreonciledDataBind"
                        hasID="true" idName="accountid" multiselect="true" footerrow="true" userDataOnFooter="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <table width="300" style="float: right; text-align: right;">
                        <tr>
                            <td>
                                Unpresented Balance</td>
                            <td>
                                <asp:Label ID="UCBL" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Actual Balance
                            </td>
                            <td>
                                <asp:Label ID="ABL" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                INSERT</div>
            <div>
                <asp:ImageButton ID="ImageButtonInsert" ImageUrl="~/images/new.gif" runat="server"
                    OnClientClick="ImageButtonInsert_ClientClick(); return false;" />
            </div>
        </div>

    </div>
</asp:Content>
