<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="cashdeposit.aspx.cs" Inherits="sapp_sms.cashdeposit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Cash Deposit</title>
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
                <asp:ImageButton ID="ImageButtonSave" runat="server" ImageUrl="Images/save.gif"                      OnClick="ImageButtonSave_Click2" />
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
        <div>
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;">
                <tr>
                    <td>
                        <b>Cash Deposit:</b>
                    </td>
                    <td colspan="3">
                        &nbsp;</td>
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
                        <asp:TextBox ID="TextBoxCashDepositRef" runat="server" Enabled="False"></asp:TextBox>
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
                        &nbsp;<b>GST:</b>
                    </td>
                    <td>
                        &nbsp;
                        <asp:CheckBox ID="GSTCK" runat="server" Checked="True" />
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
                        &nbsp;</td>
                </tr>
                <tr>
                    <td colspan="4">
                        <b __designer:mapid="f1">Description:<br />
                        <asp:TextBox ID="DescriptionT" runat="server" Height="108px" Width="564px"></asp:TextBox>
                        </b>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
