
<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="transafer.aspx.cs" Inherits="sapp_sms.transafer" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bank Transafer</title>
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
                    CausesValidation="false" OnClientClick="history.back(); return false;"  />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>From Account(*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxFromAccount" runat="server" 
                        AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                </td>
                <td>
                    <b>To Account(*):</b>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="ComboBoxToAccount" runat="server" 
                        AutoPostBack="False" DropDownStyle="DropDownList"
                        AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                        Width="150px">
                    </ajaxToolkit:ComboBox>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Reference (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxNum" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
                <td>
                    <b>Description(*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxDescription" runat="server" Width="150px"></asp:TextBox>
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
                </td>
                <td>
                    <b>Amount (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxAmount" runat="server" Width="150px"></asp:TextBox>
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
