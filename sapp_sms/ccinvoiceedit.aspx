<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="ccinvoiceedit.aspx.cs" Inherits="sapp_sms.ccinvoiceedit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - CCredit Note Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/ccinvoiceedit.js" type="text/javascript"></script>
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
        <asp:HiddenField ID="isGSTChecked" ClientIDMode="Static" runat="server" Value="No" />
        <table class="details">
            <tr>
                <td>
                    <b>Creditor Credit Note:</b>
                </td>
                <td colspan="3">
                    <b>
                        <asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="false"></asp:Label></b>
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
                    &nbsp;
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxOrder" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" Visible="False">
                    </ajaxToolkit:ComboBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Creditor(*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxCreditor" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorCreditor" runat="server" ForeColor="Red"
                        ErrorMessage="!" OnServerValidate="CustomValidatorCreditor_ServerValidate"></asp:CustomValidator>
                </td>
                <td>
                    <b>Bodycorp(*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidator1" runat="server" ForeColor="Red" ErrorMessage="!"
                        OnServerValidate="CustomValidatorBodycorp_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" Visible="False">
                    </ajaxToolkit:ComboBox>
                </td>
                <td>
                    <b></b>
                </td>
                <td>
                    <asp:CheckBox ID="CheckBoxUnitAdminFee" runat="server" Visible="false"></asp:CheckBox>
                    <asp:TextBox ID="TextBoxAdminFee" runat="server" Visible="false"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxAdminFee" ID="FilteredTextBoxExtender3"
                        runat="server" FilterType="Custom, Numbers" ValidChars=".">
                    </ajaxToolkit:FilteredTextBoxExtender>
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
                    &nbsp;
                </td>
                <td>
                    <asp:TextBox ID="TextBoxDue" runat="server" Visible="False"></asp:TextBox>
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
                    &nbsp;
                </td>
                <td>
                    <asp:TextBox ID="TextBoxApply" runat="server" Visible="False"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderApply" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxApply" />
                </td>
                <td>
                    <b style="display: none;">Paid (*):</b>
                </td>
                <td>
                    <%--<asp:TextBox ID="TextBoxPaid" runat="server" Visible="False" Text="0"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtenderPaid" runat="server" TargetControlID="TextBoxPaid" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>    
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorPaid" runat="server" 
                    ErrorMessage="!" ForeColor="Red" ControlToValidate="TextBoxPaid"></asp:RequiredFieldValidator>--%>
                </td>
            </tr>
            <tr>
                <td colspan="4">
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
                <td colspan="4">
                    <b>Purchase Order Transaction</b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart(*)','Description','Net(*)','Tax(*)','Gross(*)','GST']"
                            colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'cinvoiceedit.aspx/BindChartSelector', dataEvents: [{ type: 'focusout', fn: function(e) {$('#HiddenChart').val(this.value);} }]}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}, 
                          { name: 'Description', index: 'Description', width: 200, editable:true, editoptions: { 'maxlength': 100, dataEvents: [ { type: 'focus', fn: function(e) {this.value=$('#HiddenChart').val(); } }] }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Net', index: 'Net', width: 100,   align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Tax', index: 'Tax', width: 100,   align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Gross', index: 'Gross', width: 100, editable:true, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name:'GST',index:'GST', width:60, editable: true,edittype:'checkbox',editoptions: {value:'Yes:No', defaultValue: isGTSChecked() }}                      ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                            width="700" height="300" url="ccinvoiceedit.aspx/DataGridDataBind" hasID="false"
                            editurl="ccinvoiceedit.aspx/SaveDataFromGrid" inlineNav="true" afterRowSave="ChangeTotal"
                            footerrow="true" userDataOnFooter="true" />
                    </div>
                    <div>
                        <input type="hidden" id="HiddenNet" value="" />
                        <input type="hidden" id="HiddenNetOld" value="" />
                        <input type="hidden" id="HiddenTax" value="" />
                        <input type="hidden" id="HiddenChart" value="" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div style="width: 400px; float: left;">
                        &nbsp;
                        <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                            PostBackUrl="~/cinvoiceedit.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelNet" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelTax" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelGross" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
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
