<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="budgetmaster.aspx.cs" Inherits="sapp_sms.budgetmaster" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bodycorps</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/budgetmaster.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Save</div>
            <div>
                <asp:ImageButton ID="ImageButtonSave" runat="server" ImageUrl="Images/save.gif" CausesValidation="false"
                    PostBackUrl="~/budgetmaster.aspx" OnClientClick="return ImageButtonSave_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Add new row</div>
            <div>
                <asp:ImageButton ID="ImageButtonAddRow" runat="server" ClientIDMode="Static" ImageUrl="~/images/new.gif"
                    PostBackUrl="~/budgetmaster.aspx" OnClientClick="return ButtonAddRow_ClientClick();" />
                <asp:HiddenField ID="HiddenCMTotalAmount" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="HiddenCMCurrentAmount" runat="server" ClientIDMode="Static" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete row</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                    PostBackUrl="~/budgetmaster.aspx" OnClientClick="return ButtonDelete_ClientClick();" />
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
            <cc1:jqGridAdv runat="server" ID="jqGridBudget" colNames="['ID','Total', 'Budget Name', 'Scale', M[0], M[1], M[2], M[3], M[4],M[5],M[6],M[7],M[8],M[9],M[10],M[11]]"
                colModel="[
                      { name: 'ID', index: 'ID', width: 10, editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                      { name: 'Total', index: 'Total', width: 60, align: 'right'},
                      { name: 'Field', index: 'Field', width: 150, editable:true, edittype:'select', editoptions:{dataUrl:'budgetmaster.aspx/BindBudgetField?id='}, align: 'left'},
                      { name: 'Scale', index: 'Scale', width: 45, editable:true, edittype:'select', editoptions:{dataUrl:'budgetmaster.aspx/BindScale'}, align: 'left'},
                      { name: 'M1', index: 'M1', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M2', index: 'M2', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M3', index: 'M3', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M4', index: 'M4', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M5', index: 'M5', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M6', index: 'M6', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M7', index: 'M7', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M8', index: 'M8', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M9', index: 'M9', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M10', index: 'M10', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M11', index: 'M11', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}},
                      { name: 'M12', index: 'M12', width: 45, editable:true, editoptions: { 'maxlength': 10 }, align: 'right', cellattr: setBackgroundColor, editrules:{custom:true, custom_func:ValidateRowData}}
                  ]" rowNum="10000" rowList="[]" viewrecords="true"
                multiselect="true" width="800" height="600" url="budgetmaster.aspx/DataGridDataBind"
                hasID="false" editurl="budgetmaster.aspx/SaveDataFromGrid" inlineNav="true" afterRowSave="SaveDataGrid"
                footerrow="true" userDataOnFooter="true" onRowClientSelect="onRowClientSelect" />
            <%--<asp:HiddenField ID="HiddenCurValue" runat="server" ClientIDMode="Static" />--%>
            <asp:HiddenField ID="HiddenUsedFlgs" runat="server" ClientIDMode="Static" />
        </div>
        <asp:Panel ID="PanelFillValue" CssClass="PopupCSS" runat="server">
            <div>
                <b>Fill Value:</b><asp:TextBox ID="TextBoxValue" runat="server" ClientIDMode="Static"></asp:TextBox>
            </div>
            <br />
            <div>
                <asp:Button ID="ButtonOK" runat="server" Text="OK" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" />
            </div>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="MPE" runat="server" TargetControlID="ImageButtonFill"
            PopupControlID="PanelFillValue" BackgroundCssClass="modalBackground" DropShadow="true"
            OkControlID="ButtonOK" OnOkScript="OnFillClick()" CancelControlID="ButtonCancel">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="PanelSplit" CssClass="PopupCSS" runat="server">
            <div>
                <b>Total Amount:</b><asp:TextBox ID="TextBoxTotal" runat="server" ClientIDMode="Static"></asp:TextBox>
            </div>
            <br />
            <div>
                <asp:Button ID="ButtonSplitOk" runat="server" Text="OK" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="ButtonSplitCancel" runat="server" Text="Cancel" />
            </div>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="MPE2" runat="server" TargetControlID="ImageButtonSplit"
            PopupControlID="PanelSplit" BackgroundCssClass="modalBackground" DropShadow="true"
            OkControlID="ButtonSplitOk" OnOkScript="OnSplitClick()" CancelControlID="ButtonSplitCancel">
        </ajaxToolkit:ModalPopupExtender>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Fill</div>
            <div>
                <asp:ImageButton ID="ImageButtonFill" runat="server" ImageUrl="Images/fill.gif" CausesValidation="false" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Split</div>
            <div>
                <asp:ImageButton ID="ImageButtonSplit" runat="server" ImageUrl="Images/check.gif"
                    CausesValidation="false" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Export</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/check.gif" CausesValidation="false"
                    OnClick="ImageButton1_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Import</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/check.gif" CausesValidation="false"
                    OnClick="ImageButton2_Click" />
            </div>
        </div>
    </div>
</asp:Content>
