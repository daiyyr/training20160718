<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="invoicemasteredit.aspx.cs" Inherits="sapp_sms.invoicemasteredit" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<%@ Register src="UseControl/ChartCodeSearch.ascx" tagname="ChartCodeSearch" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Invoice Master Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/invoicemasteredit.js" type="text/javascript"></script>
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
        <asp:HiddenField ID="isLimitedEdit" ClientIDMode="Static" runat="server" Value="false" />
        <asp:HiddenField ID="isGSTChecked" ClientIDMode="Static" runat="server" Value="No" />
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
                    <b>Bodycorp (*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" AutoPostBack="True">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidatorBodycorp" runat="server" ForeColor="Red"
                        ErrorMessage="!" OnServerValidate="CustomValidatorBodycorp_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Unit :</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" onselectedindexchanged="ComboBoxUnit_SelectedIndexChanged">
                    </ajaxToolkit:ComboBox>
                </td>
                <td>
                    <b>Debtor (*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxDebtor" runat="server" AutoPostBack="True" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px" OnSelectedIndexChanged="ComboBoxDebtor_SelectedIndexChanged">
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
                    <b>Invoice Date (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxDate" runat="server" ClientIDMode="Static" OnChange="OnInvoiceDateChange()"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxDate">
                    </ajaxToolkit:CalendarExtender>
                    
                    <asp:CustomValidator ID="CustomValidatorInvoiceDate" runat="server" ForeColor="Red" ErrorMessage="invoice date must be not before bodycorp's begin date"
                        OnServerValidate="CustomValidatorInvoiceDate_ServerValidate"></asp:CustomValidator>
                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxDate"></asp:RequiredFieldValidator>--%>
                    <asp:HiddenField ID="HiddenFieldBaseDateBkn" ClientIDMode="Static" runat="server" Value="" />
                </td>
                <td>
                    <b>Due Date:</b></td>
                <td>
                    <asp:TextBox ID="TextBoxDue" runat="server" ClientIDMode="Static" OnChange="OnDueDateChange()"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderDue" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxDue" />
                    <asp:CustomValidator ID="CustomValidatorDueDate" runat="server" ForeColor="Red" ErrorMessage="due date must be not before invoice date"
                        OnServerValidate="CustomValidatorDueDate_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Apply Date:</b>
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TextBoxApply" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderApply" CssClass="sappcalendar" Format="dd/MM/yyyy" TargetControlID="TextBoxApply" />
                    <asp:CustomValidator ID="CustomValidatorApplyDate" runat="server" ForeColor="Red" ErrorMessage="apply date must be not before invoice date and not after due date"
                        OnServerValidate="CustomValidatorApplyDate_ServerValidate"></asp:CustomValidator>
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
                    <b>GL Transaction</b>
                    <br />
                    <asp:HiddenField ID="hidenChartCodeName" ClientIDMode="Static" runat="server" Value=""/>
                    <asp:DropDownList ID="DropDownListType" ClientIDMode="Static" runat="server">
                        <asp:ListItem>Chart Name</asp:ListItem>
                    </asp:DropDownList>
                    <input id="TxtChartName" name="TxtChartName" type="text" value="" style="width:160px;" />
                    <button id="btnSearchChartData" name="btnSearchChartData" runat="server" type="button" onclick="searchChartData();">Search</button>
                    <span id="LblChartMasterSearchResult"></span>
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart(*)','Description','Net(*)','Tax(*)','Gross(*)','GST']"
                            colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'invoicemasteredit.aspx/BindChartSelector', dataEvents: [{ type: 'focusout', fn: function(e) {$('#HiddenChart').val(this.value);} }]}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 200, editable:true, editoptions: { 'maxlength': 100, dataEvents: [ { type: 'focus', fn: function(e) {this.value=$('#HiddenChart').val(); } }] }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Net', index: 'Net', width: 100,   align: 'left', editable:isCellEditable(), search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Tax', index: 'Tax', width: 100,   align: 'left', editable:isCellEditable(), search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Gross', index: 'Gross', width: 100, editable:isCellEditable(), align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          {name:'GST',index:'GST', width:60, editable:isCellEditable(), edittype:'checkbox',editoptions: {value:'Yes:No', defaultValue: isGTSChecked() }}
                     ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                            width="700" height="300" url="invoicemasteredit.aspx/DataGridDataBind" hasID="false"
                            editurl="invoicemasteredit.aspx/SaveDataFromGrid" inlineNav="true" afterRowSave="ChangeTotal"
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
                            PostBackUrl="~/invoicemasteredit.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
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
