
<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="journalsedit.aspx.cs" Inherits="sapp_sms.Journalsedit1" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Journal Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/Jouranledit.js" type="text/javascript"></script>
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
                    <b>Bodycorp(*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                    <asp:CustomValidator ID="CustomValidator1" runat="server" ForeColor="Red" ErrorMessage="!"
                        OnServerValidate="CustomValidatorBodycorp_ServerValidate"></asp:CustomValidator>
                </td>
                <td>
                    <b>Num (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxNum" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNum" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxNum"></asp:RequiredFieldValidator>
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
                    <b>
                        <asp:Label ID="UnitL" runat="server" Text="Unit:"></asp:Label></b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td colspan="3">
                    <asp:TextBox TextMode="MultiLine" ID="TextBoxDescription" runat="server" Width="500px"
                        Visible="False"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <b>GL Transaction</b>&nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart(*)','Description','Debit','Credit']"
                            colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'Journalsedit.aspx/BindChartSelector', dataEvents: [{ type: 'focusout', fn: function(e) {$('#HiddenChart').val(this.value);} }]}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 200, editable:true, editoptions: { 'maxlength': 100, dataEvents: [ { type: 'focus', fn: function(e) {this.value=$('#HiddenChart').val(); } }] }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Debit', index: 'Debit', width: 100, editable:true, align: 'left', search: true,editrules:{custom:true, custom_func:ValidateRowData}}, 
                          { name: 'Credit', index: 'Creidt', width: 100, editable:true,  align: 'left',editrules:{custom:true, custom_func:ValidateRowData}},
                      ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                            width="700" height="300" url="journalsedit.aspx/DataGridDataBind" hasID="false"
                            editurl="journalsedit.aspx/SaveDataFromGrid" inlineNav="true" footerrow="true"
                            userDataOnFooter="true" />
                    </div>
                    <div>
                        <input type="hidden" id="HiddenNet" value="" />
                        <input type="hidden" id="HiddenNetOld" value="" />
                        <input type="hidden" id="HiddenChart" value="" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div style="width: 400px; float: left;">
                        <img id="DeleteI" alt="" src="images/delete.gif" /></div>
                    <script>
                        $("#DeleteI").click(function () {

                            if (confirm("Are you sure you want to delete the item?") == true) {
                                var grid = jQuery("#" + GetClientId("jqGridTrans") + "_datagrid1");
                                var rowKey = grid.getGridParam("selrow");
                                var id = grid.jqGrid('getCell', rowKey, 'ID');
                                $.ajax({

                                    type: "Post",
                                    contentType: "application/json;utf-8",
                                    url: "Journalsedit.aspx/DeleteRow",
                                    data: "{rowID:'" + id + "'}",
                                    dataType: "json",
                                    success: function (result) {
                                        grid.trigger("reloadGrid");
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        alert(XMLHttpRequest.status);
                                        alert(XMLHttpRequest.readyState);
                                        alert(textStatus);
                                    }
                                });
                            }


                        });
                       
                    </script>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelNet" runat="server" ForeColor="#006600" Visible="false" Text=""></asp:Label>
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelTax" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelGross" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
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
    <script>
        function IsDecimal(value) {
            var regex = /^(\+|-)?([0-9]*\.?[0-9]*)$/;
            if (regex.test(value))
                return true;
            else
                return false;
        }
        function ValidateRowData(value, colname) {
            //            if (value == "") {
            //                return [false, colname + ' Required'];
            //            }
            //            else {
            if (colname == "Debit") {
                if (!IsDecimal(value)) {
                    return [false, colname + 'should be decimal'];
                }
            }
            else if (colname == "Credit") {
                if (!IsDecimal(value)) {
                    return [false, colname + 'should be decimal'];
                }
            }
            else {
                return [true, ''];
            }
            return [true, ''];

        }
    </script>
</asp:Content>
