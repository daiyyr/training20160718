<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="purchordermasteredit.aspx.cs" Inherits="sapp_sms.purchordermasteredit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Purchase Order Master Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/purchordermasteredit.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            width: 93px;
        }
    </style>
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
                    CausesValidation="false" OnClientClick="history.back(); return false;" OnClick="ImageButtonClose_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details" style="width: 750px">
            <tr>
                <td colspan="2">
                    <b>Element ID:</b>
                </td>
                <td colspan="6">
                    <b>
                        <asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <b>Num (*):</b>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="TextBoxNum" runat="server" Enabled="False"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNum" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxNum"></asp:RequiredFieldValidator>
                </td>
                <td colspan="2">
                    <b>Type (*):</b>
                </td>
                <td colspan="2">
                    <ajaxToolkit:ComboBox ID="ComboBoxType" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorType" runat="server" ForeColor="Red" ErrorMessage="!"
                        OnServerValidate="CustomValidatorType_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <b>Creditor(*):</b>
                </td>
                <td colspan="2">
                    <ajaxToolkit:ComboBox ID="ComboBoxCreditor" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorCreditor" runat="server" ForeColor="Red"
                        ErrorMessage="!" OnServerValidate="CustomValidatorCreditor_ServerValidate"></asp:CustomValidator>
                </td>
                <td colspan="2">
                    <b>Bodycorp(*):</b>
                </td>
                <td colspan="2">
                    <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorCreditor0" runat="server" ForeColor="Red"
                        ErrorMessage="!" OnServerValidate="CustomValidatorBodycorp_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <b>Unit :</b>
                </td>
                <td colspan="2">
                    <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                </td>
                <td colspan="2">
                    <b>Date (*):</b>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxDate" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxDate"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                </td>
            </tr>
            <tr style="display: none;">
                <td colspan="2">
                    <b>Approval :</b>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="TextBoxApproval" runat="server"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtenderApproval" runat="server"
                        TargetControlID="TextBoxApproval" FilterType="Numbers">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
                <td colspan="2">
                    &nbsp;
                </td>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr style="display: none;">
                <td colspan="8">
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <b>Net:</b>
                </td>
                <td colspan="2">
                    <asp:Label ID="LabelNet" runat="server" ForeColor="#006600"></asp:Label>
                </td>
                <td class="style1">
                    <b>Tax:</b>
                </td>
                <td class="style1">
                    <asp:Label ID="LabelTax" runat="server" ForeColor="#006600"></asp:Label>
                </td>
                <td colspan="2">
                    <b>Gross:</b>
                </td>
                <td class="style1">
                    <asp:Label ID="LabelGross" runat="server" ForeColor="#006600"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <b>Note:</b>
                </td>
                <td colspan="6">
                    <asp:TextBox ID="TextBoxNote" runat="server" TextMode="MultiLine" Width="500px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <b>Purchase Order Transaction</b>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart(*)','Description','Net(*)','Tax(*)','Gross(*)']"
                            colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'purchordermasteredit.aspx/BindChartSelector', dataEvents: [{ type: 'focusout', fn: function(e) {$('#HiddenChart').val(this.value);} }]}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 200, editable:true, editoptions: { 'maxlength': 100, dataEvents: [ { type: 'focus', fn: function(e) {this.value=$('#HiddenChart').val(); } }] }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Net', index: 'Net', width: 100, editable:true, editoptions: { dataEvents: [{ type: 'focus', fn: function(e) {$('#HiddenNetOld').val(this.value);} }, { type: 'focusout', fn: function(e) {$('#HiddenNet').val(this.value);} }] }, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Tax', index: 'Tax', width: 100, editable:true, editoptions: { dataEvents: [ { type: 'focus', fn: function(e) {this.value=((parseFloat($('#HiddenNet').val())*15)*0.01).toFixed(2); } }, { type: 'focusout', fn: function(e) {$('#HiddenTax').val(this.value);} }] }, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Gross', index: 'Gross', width: 100, editable:true, editoptions: { dataEvents: [ { type: 'focus', fn: function(e) {this.value=(parseFloat($('#HiddenNet').val()) + parseFloat($('#HiddenTax').val())).toFixed(2); } }] }, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                      ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                            width="700" height="500" url="purchordermasteredit.aspx/DataGridDataBind" hasID="false"
                            editurl="purchordermasteredit.aspx/SaveDataFromGrid" inlineNav="true" />
                    </div>
                    <div>
                        <input type="hidden" id="HiddenNet" value="" />
                        <input type="hidden" id="HiddenNetOld" value="" />
                        <input type="hidden" id="HiddenTax" value="" />
                        <input type="hidden" id="HiddenChart" value="" />
                        <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                            PostBackUrl="~/purchordermasteredit.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
                    </div>
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
