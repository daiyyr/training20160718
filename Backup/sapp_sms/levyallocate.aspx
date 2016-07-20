<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="levyallocate.aspx.cs" Inherits="sapp_sms.levyallocate" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Levy Allocate</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="scripts/levyallocate.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
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
        <table class="details">
            <tr>
                <td>
                    <b>Bodycorp (*):</b>
                </td>
                <td>
                    <asp:Label ID="LabelBodycorp" runat="server" Text="Label" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <b>Billing Date (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxBillingDate" runat="server" ClientIDMode="Static" Height="18px" onchange="OnBillingDateChange()">01</asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxBillingDate"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                </td>
                <td>
                    <b>Due Date(*)</b>
                </td>
                <td style="vertical-align: middle">
                    <asp:TextBox ID="TextBoxDueM" runat="server" ClientIDMode="Static" 
                        Height="18px" Width="50px" onchange="OnDueDateChange()">0</asp:TextBox>
                    M<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxDueD"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="TextBoxDueD" runat="server" ClientIDMode="Static" 
                        Height="18px" Width="50px" onchange="OnDueDateChange()">20</asp:TextBox>
                    D<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxDueM"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                </td>
                <td>
                    <b>Apply Date(*)</b>
                </td>
                <td style="vertical-align: middle">
                    <asp:TextBox ID="TextBoxApplyDateM" runat="server" ClientIDMode="Static" Height="18px" Width="50px" MaxLength="2"></asp:TextBox>
                    M<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="!" ForeColor="Red" ControlToValidate="TextBoxApplyDateM"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="TextBoxApplyDateD" runat="server" ClientIDMode="Static" Height="18px" Width="50px" MaxLength="2"></asp:TextBox>
                    D<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="!" ForeColor="Red" ControlToValidate="TextBoxApplyDateD"></asp:RequiredFieldValidator>
                    <asp:HiddenField ID="HiddenFieldBaseDateBkn" ClientIDMode="Static" runat="server" Value="" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Description (*):</b>
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TextBoxDescription" runat="server" TextMode="MultiLine" Width="600px"
                        Height="100px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorDescription" runat="server"
                        ErrorMessage="!" ForeColor="Red" ControlToValidate="TextBoxDescription"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Consolidate:</b>
                </td>
                <td>
                    <asp:CheckBox ID="CheckBoxConsolidate" runat="server" Checked="true" />
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
        </table>
        <div style="width: 100%">
            <img src="Images/dot.gif" height="4px" />
            <asp:Panel ID="Panel1" runat="server" Width="810" Height="600" ScrollBars="Auto">
                <cc1:jqGridAdv runat="server" ID="jqGridLevy" colNames="['ID', 'Chart', 'Budget Name', 'Total', 'Scale', M[0], M[1], M[2], M[3], M[4],M[5],M[6],M[7],M[8],M[9],M[10],M[11], M[12], M[13],M[14],M[15],M[16],M[17],M[18],M[19],M[20],M[21],M[22],M[23]]"
                    colModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                        { name: 'Chart', index: 'Chart', width: 100, editable:false, align: 'left', hidden:true},
                        { name: 'Field', index: 'Field', width: 100, editable:false, align: 'left'},
                        { name: 'Total', index: 'Total', width: 50, editable:false, align: 'left'},
                        { name: 'Scale', index: 'Scale', width: 45, editable:false,  align: 'left', hidden:true},
                        { name: 'M1', index: 'M1', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M2', index: 'M2', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M3', index: 'M3', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M4', index: 'M4', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M5', index: 'M5', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M6', index: 'M6', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M7', index: 'M7', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M8', index: 'M8', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M9', index: 'M9', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M10', index: 'M10', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M11', index: 'M11', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M12', index: 'M12', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M13', index: 'M13', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M14', index: 'M14', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M15', index: 'M15', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M16', index: 'M16', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M17', index: 'M17', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M18', index: 'M18', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M19', index: 'M19', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M20', index: 'M20', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M21', index: 'M21', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M22', index: 'M22', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M23', index: 'M23', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                        { name: 'M24', index: 'M24', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'left', hidden:true},
                    ]" rowNum="10000" rowList="[]" sortname="ID" sortorder="asc" viewrecords="true"
                    multiselect="true" width="800" height="500" url="levyallocate.aspx/DataGridDataBind"
                    hasID="false" editurl="levyallocate.aspx/SaveDataFromGrid" inlineNav="true" afterRowSave="SaveDataGrid"
                    additionalScripts="" />
            </asp:Panel>
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Fill</div>
            <div>
                <asp:ImageButton ID="ImageButtonFill" runat="server" ImageUrl="Images/fill.gif" CausesValidation="false"
                    OnClientClick="OnFillClick(); return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Submit</div>
            <div>
                <asp:ImageButton ID="ImageButtonSubmit" runat="server" ImageUrl="~/images/submit.gif"
                    OnClientClick="return ImageButtonSave_ClientClick()" />
            </div>
        </div>
    </div>
</asp:Content>
