<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="creditornotesedit.aspx.cs" Inherits="sapp_sms.creditornotesedit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Invoice Master Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/creditornotesedit.js" type="text/javascript"></script>
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
        <table class="details">
            <tr>
                <td>
                    <b>Element ID:</b>
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
                    <b>Num (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxNum" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNum" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxNum"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b>Debtor(*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxDebtor" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorDebtor" runat="server" ForeColor="Red" ErrorMessage="!"
                        OnServerValidate="CustomValidatorDebtor_ServerValidate"></asp:CustomValidator>
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
                    <b>Unit :</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
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
                    <b>Due</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxDue" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderDue" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxDue" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Description</b>
                </td>
                <td colspan="3">
                    <asp:TextBox TextMode="MultiLine" ID="TextBoxDescription" runat="server" Width="500px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>BatchID :</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxBatchID" runat="server"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server"
                        TargetControlID="TextBoxBatchID" FilterType="Numbers">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
                <td>
                    <b>Net:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxNet" runat="server" ReadOnly="True"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
                        TargetControlID="TextBoxNet" FilterType="Custom, Numbers" ValidChars=".">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Tax:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxTax" runat="server" ReadOnly="True"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtenderTax" runat="server"
                        TargetControlID="TextBoxTax" FilterType="Custom, Numbers" ValidChars=".">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
                <td>
                    <b>Gross:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxGross" runat="server" ReadOnly="True"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtenderGross" runat="server"
                        TargetControlID="TextBoxGross" FilterType="Custom, Numbers" ValidChars=".">
                    </ajaxToolkit:FilteredTextBoxExtender>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Paid (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxPaid" runat="server"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtenderPaid" runat="server"
                        TargetControlID="TextBoxPaid" FilterType="Custom, Numbers" ValidChars=".">
                    </ajaxToolkit:FilteredTextBoxExtender>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorPaid" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxPaid"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b></b>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <b>GL Transaction</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart(*)','Description','Net(*)','Tax(*)','Gross(*)']"
                            colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'purchordermasteredit.aspx/BindChartSelector', dataEvents: [{ type: 'focusout', fn: function(e) {$('#HiddenChart').val(this.value);} }]}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 200, editable:true, editoptions: { 'maxlength': 100, dataEvents: [ { type: 'focus', fn: function(e) {this.value=$('#HiddenChart').val(); } }] }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Net', index: 'Net', width: 100, editable:true, editoptions: { dataEvents: [{ type: 'focus', fn: function(e) {$('#HiddenNetOld').val(this.value);} }, { type: 'focusout', fn: function(e) {$('#HiddenNet').val(this.value);ChangeNet($('#HiddenNetOld').val(),this.value);} }] }, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Tax', index: 'Tax', width: 100, editable:true, editoptions: { dataEvents: [ { type: 'focus', fn: function(e) {this.value=((parseFloat($('#HiddenNet').val())*15)*0.01).toFixed(2); } }] }, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Gross', index: 'Gross', width: 100, editable:true, editoptions: { dataEvents: [ { type: 'focus', fn: function(e) {this.value=(parseFloat($('#HiddenNet').val()) + parseFloat($('#HiddenTax').val())).toFixed(2); } }] }, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                      ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                            width="700" height="300" url="creditornotesedit.aspx/DataGridDataBind" hasID="false"
                            multiselect="true" />
                    </div>
                    <div>
                        <input type="hidden" id="HiddenNet" value="" />
                        <input type="hidden" id="HiddenNetOld" value="" />
                        <input type="hidden" id="HiddenTax" value="" />
                        <input type="hidden" id="HiddenChart" value="" />
                        <asp:Label ID="LabelSelectedRows" runat="server" Text=""></asp:Label>
                        <asp:Button ID="ButtonCommand" runat="server" Text="Command" PostBackUrl="~/cpaymentedit.aspx"
                            OnClientClick="return ImageButtonCommand_ClientClick()" />
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
