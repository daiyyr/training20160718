<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bankreconciliation3.aspx.cs" Inherits="sapp_sms.bankreconciliation3" %>

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
    <style type="text/css">
        .style1
        {
            height: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
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
                    <img src="Images/dot.gif" height="4px" /><div>
                        <b>&nbsp;Transtions<asp:HiddenField ID="account_id_HF" runat="server" />
                        </b>
                        <asp:HiddenField ID="cutoffdate_HF" runat="server" />
                    </div>
                    <cc1:jqgridadv runat="server" id="jqGridUnreconciled" colnames="['ID','Date','CD','Ref','Description','Deposit', 'Payment']"
                        colmodel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
               
           { name: 'Date', index: 'Date', width: 150, align: 'left', search: true, formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}},
            { name: 'CD', index: 'CD', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
        { name: 'Ref', index: 'Ref', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  
                         { name: 'Description', index: 'Description', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Deposit', index: 'Deposit', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Payment', index: 'Payment', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rownum="10000" rowlist="[]" sortname="Date" sortorder="asc" viewrecords="true"
                        width="700" height="300" url="bankreconciliation2.aspx/jqGridUnreonciledDataBind"
                        hasid="true" idname="accountid" multiselect="true" footerrow="true" userdataonfooter="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <table width="300" style="float: right">
                        <tr>
                            <td class="style1">
                                &nbsp;Closing Balance
                            </td>
                            <td class="style1">
                                <asp:Label ID="CBL" runat="server" Text="0"></asp:Label>
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
                Report</div>
            <div>
                <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank">
                    <asp:Image ID="ImageButton2" runat="server" CausesValidation="false" ImageUrl="Images/Maintenance.gif" /></asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>
