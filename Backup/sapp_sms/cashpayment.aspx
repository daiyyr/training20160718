<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="cashpayment.aspx.cs" Inherits="sapp_sms.cashpayment" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Cash Payment</title>
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
            <img src="Images/dot.gif" height="4px" />
            <%--<asp:HiddenField ID="HiddenCurValue" runat="server" ClientIDMode="Static" />--%>
            <table style="width: 600px; border-width: 2px 2px 2px 2px; border-spacing: 1px; border-style: solid solid solid solid;
                border-color: #a6c9e2 #a6c9e2 #a6c9e2 #a6c9e2; border-collapse: separate; text-align: left;
                background-color: White;" __designer:mapid="d4">
                <tr __designer:mapid="d5">
                    <td __designer:mapid="d6">
                        <b __designer:mapid="d7">Cash Payment:</b>
                    </td>
                    <td colspan="3" __designer:mapid="d8">
                        &nbsp;
                    </td>
                </tr>
                <tr __designer:mapid="db">
                    <td colspan="4" __designer:mapid="dc">
                    </td>
                </tr>
                <tr __designer:mapid="dd">
                    <td __designer:mapid="de">
                        <b __designer:mapid="df">Ref(*):</b>
                    </td>
                    <td __designer:mapid="e0">
                        <asp:TextBox ID="TextBoxCashPaymentRef" runat="server" Enabled="False"></asp:TextBox>
                    </td>
                    <td __designer:mapid="e2">
                        <b __designer:mapid="e3">Chart(*):</b>
                    </td>
                    <td __designer:mapid="e4">
                        <ajaxtoolkit:combobox id="ChartCashPaymentCombobox" runat="server" dropdownstyle="DropDownList"
                            autocompletemode="SuggestAppend" casesensitive="False" iteminsertlocation="Append"
                            width="150px">
                        </ajaxtoolkit:combobox>
                    </td>
                </tr>
                <tr __designer:mapid="e6">
                    <td __designer:mapid="e7">
                        <b __designer:mapid="e8">Bodycorp:</b>&nbsp;
                    </td>
                    <td __designer:mapid="e9">
                        <ajaxtoolkit:combobox id="ComboBoxCashPaymentBodycorp" runat="server" autocompletemode="SuggestAppend"
                            autopostback="False" casesensitive="False" dropdownstyle="DropDownList" iteminsertlocation="Append"
                            width="150px">
                        </ajaxtoolkit:combobox>
                    </td>
                    <td __designer:mapid="eb">
                        <b __designer:mapid="e3">GST:</b>
                    </td>
                    <td __designer:mapid="ec">
                        &nbsp;
                        <asp:CheckBox ID="GSTCK" runat="server" Checked="True" />
                    </td>
                </tr>
                <tr __designer:mapid="ed">
                    <td colspan="4" __designer:mapid="ee">
                    </td>
                </tr>
                <tr __designer:mapid="ef">
                    <td __designer:mapid="f0">
                        <b __designer:mapid="f1">Date (*):</b>
                    </td>
                    <td __designer:mapid="f2">
                        <asp:TextBox ID="TextBoxCashPaymentDate" runat="server"></asp:TextBox>
                        <ajaxtoolkit:calendarextender runat="server" id="CalendarDate1" format="dd/MM/yyyy"
                            cssclass="sappcalendar" targetcontrolid="TextBoxCashPaymentDate" />
                    </td>
                    <td __designer:mapid="f5">
                        <b __designer:mapid="f6">Amount(*):</b>
                    </td>
                    <td __designer:mapid="f7">
                        <asp:TextBox ID="TextBoxCashPaymentGross" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr __designer:mapid="f9">
                    <td colspan="4" __designer:mapid="fa">
                        &nbsp;</td>
                </tr>
                <tr __designer:mapid="f9">
                    <td colspan="4" __designer:mapid="fa">
                        <b __designer:mapid="f1">Description:</b>
                        <br />
                        <asp:TextBox ID="DescriptionT" runat="server" Height="108px" Width="564px"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
