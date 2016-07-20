<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="levylist.aspx.cs" Inherits="sapp_sms.levylist" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Levy List</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="scripts/levylist.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Levies</div>
            <div>
                <asp:ImageButton ID="ImageButtonLevies" runat="server" ImageUrl="Images/levies.gif"
                    CausesValidation="false" OnClick="ImageButtonLevies_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false" OnClientClick="window.close(); return false;" />
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
                <td colspan="2">
                    <table>
                        <tr>
                            <td>
                                <b>Begin (*):</b>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBoxDate" runat="server" ClientIDMode="Static" Height="18px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDate" CssClass="sappcalendar"
                                    Format="dd/MM/yyyy" TargetControlID="TextBoxDate" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" ErrorMessage="!"
                                    ForeColor="Red" ControlToValidate="TextBoxDate"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <b>End (*)</b>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBoxEnd" runat="server" ClientIDMode="Static" Height="18px"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderEnd" CssClass="sappcalendar"
                                    Format="dd/MM/yyyy" TargetControlID="TextBoxEnd" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorEnd" runat="server" ErrorMessage="!"
                                    ForeColor="Red" ControlToValidate="TextBoxEnd"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label ID="InterestRateL" runat="server"></asp:Label>
                    <asp:CheckBox ID="CheckBox1" runat="server" Text="Add Interest" />
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridLevyList" colNames="['ID','Chart(*)','Description(*)','Net(*)','Scale']"
                            colModel="[
                              { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                             

                              { name: 'Chart', index: 'Chart', width: 50, editable:true, edittype:'select', editoptions:{dataUrl:'levylist.aspx/BindChartSelector', dataEvents: [{ type: 'focusout', fn: function(e) {$('#HiddenChart').val(this.value);} }]}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 200, editable:true, editoptions: { 'maxlength': 100, dataEvents: [ { type: 'focus', fn: function(e) {this.value=$('#HiddenChart').val(); } }] }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                              { name: 'Net', index: 'Net', width: 50, editable:true, editrules:{custom:true, custom_func:DecimalNotNull}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                              { name: 'Scale', index: 'Scale', width: 50, editable:true, edittype:'select', editoptions:{dataUrl:'levylist.aspx/BindScaleSelector'},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          ]" rowNum="10000" rowList="[]" sortname="ID" sortorder="asc" viewrecords="true"
                            width="800" height="500" url="levylist.aspx/LevyListDataBind" hasID="false" editurl="levylist.aspx/SaveDataFromGrid"
                            inlineNav="true" afterRowSave="ChangeTotal" footerrow="true" userDataOnFooter="true" />
                        <input type="hidden" id="HiddenChart" value="" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div style="width: 400px; float: left;">
                        <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                            PostBackUrl="~/levylist.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelTotalAmount" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
        <asp:Panel ID="PanelInstallment" CssClass="PopupCSS" runat="server">
            <div>
                <table class="modaltable">
                    <tr>
                        <td>
                            How many installment:
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownListInstallment" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="ButtonOK" runat="server" Text="OK" />
                            <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="MPE2" runat="server" TargetControlID="ImageButtonSubmit"
            PopupControlID="PanelInstallment" BackgroundCssClass="modalBackground" DropShadow="true"
            OkControlID="ButtonOK" OnOkScript="ButtonOKClick()" CancelControlID="ButtonCancel">
        </ajaxToolkit:ModalPopupExtender>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Next Step</div>
            <div>
                <asp:ImageButton ID="ImageButtonSubmit" runat="server" ImageUrl="~/images/goright.gif" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Budget Levy</div>
            <div>
                <asp:ImageButton ID="ImageButtonBudget" runat="server" ImageUrl="~/images/budget.gif"
                    CausesValidation="false" OnClick="ImageButtonBudget_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Clear Budget</div>
            <div>
                <asp:ImageButton ID="ImageButtonClear" runat="server" ImageUrl="~/images/delete_all.gif"
                    CausesValidation="false" Enabled="False" OnClick="ImageButtonClear_Click" />
            </div>
        </div>
    </div>
</asp:Content>
