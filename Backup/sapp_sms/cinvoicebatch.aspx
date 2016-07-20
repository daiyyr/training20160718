<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="cinvoicebatch.aspx.cs" Inherits="sapp_sms.cinvoicebatch" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Cinvoices</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <style type="text/css">
        .ajax__combobox_buttoncontainer button
        {
            background-image: url(mvwres://AjaxControlToolkit, Version=4.1.60919.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e/ComboBox.arrow-down.gif);
            background-position: center;
            background-repeat: no-repeat;
            border-color: ButtonFace;
            height: 15px;
            width: 15px;
        }
        .ajax__combobox_buttoncontainer button
        {
            background-image: url(mvwres://AjaxControlToolkit, Version=4.1.60919.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e/ComboBox.arrow-down.gif);
            background-position: center;
            background-repeat: no-repeat;
            border-color: ButtonFace;
            height: 15px;
            width: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Save</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="Images/save.gif" OnClick="ImageButtonEdit_Click1" />
            </div>
        </div>
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
        <asp:Panel ID="Panel1" runat="server">
            <table class="details">
                <tr>
                    <td>
                        Type:<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                            ControlToValidate="ComboBoxType"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="ComboBoxType" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                    </td>
                    <td>
                        Amount
                    </td>
                    <td>
                        <asp:TextBox ID="AmountT" runat="server"></asp:TextBox>
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Submit" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server" Visible="False">
            <table class="details">
                <tr>
                    <td>
                        Type:
                        <asp:Label ID="TypeL" runat="server"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Amount:&nbsp;
                        <asp:Label ID="AmountL" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div>
                            <img src="Images/dot.gif" height="4px" />
                            <cc1:jqGridAdv ID="jqGridTable" runat="server" colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                 { name: 'Num', index: 'Num', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Order', index: 'Order', width: 150, align: 'left',  search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Creditor', index: 'Creditor', width: 150, align: 'left',  search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Bodycorp', index: 'Bodycorp', width: 150, align: 'left',  search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Unit', index: 'Unit', width: 150, align: 'left',  search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Date', index: 'Date', width: 150, align: 'left',search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Due', index: 'Due', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Gross', index: 'Gross', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},    
                                  { name: 'Paid', index: 'Paid', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                                                           { name: 'Balance', index: 'Balance', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                                                                { name: 'Pay',editable:true , index: 'Paid', width: 150, align: 'left',search: true, searchoptions: { sopt: ['cn', 'nc']}}
                 ]" colNames="['ID', 'Num', 'Order','Creditor','Bodycorp','Unit','Date','Due','Gross','Paid','Balance','Pay']"
                                editurl="cinvoicebatch.aspx/SaveDataFromGrid" hasID="false" height="500" inlineNav="true"
                                rowList="[5, 10, 25, 50, 100]" rowNum="25" sortname="ID" sortorder="asc" url="cinvoicebatch.aspx/DataGridDataBind"
                                viewrecords="true" width="800" userDataOnFooter="true" footerrow="true" />
                        </div>
                    </td>
                </tr>
            </table>
            <script type="text/javascript">
                function JGAjaxErrorFunction() {
                    var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                    grid.trigger("reloadGrid");
                }
            </script>
        </asp:Panel>
    </div>
</asp:Content>
