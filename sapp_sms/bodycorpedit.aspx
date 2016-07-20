<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bodycorpedit.aspx.cs" Inherits="sapp_sms.bodycorpedit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - BodyCorp Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/jquery-ui-timepicker-addon.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.core.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.widget.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.mouse.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.datepicker.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.slider.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <%--<script src="Scripts/common.js" type="text/javascript"></script>--%>
    <script src="scripts/bodycorpedit.js" type="text/javascript"></script>
    <script type="text/javascript">
    </script>
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
                    <b>Bodycorp:</b>
                </td>
                <td colspan="3">
                    <b>
                        <asp:Label ID="LabelBodycorpID" runat="server" Text="ID" Visible="false"></asp:Label></b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Code(*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxCode" ClientIDMode="Static" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorCode" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxCode"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b>Name(*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxName" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorNumber" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxName"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>GST:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxGST" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
                <td>
                    <b>GST Registered:</b>
                </td>
                <td>
                    <asp:CheckBox ID="CheckBoxGST" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                    <b>Discount:</b>
                </td>
                <td>
                    <asp:CheckBox ID="CheckBoxDiscount" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Bank Account (*):</b>
                </td>
                <td>
                    <div>
                        <ajaxToolkit:ComboBox ID="ComboBoxAccount" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                        <asp:CustomValidator ID="CustomValidatorAccount" runat="server" ForeColor="Red" ErrorMessage="!"
                            OnServerValidate="CustomValidatorAccount_ServerValidate"></asp:CustomValidator>
                    </div>
                    <div>
                        <asp:Image ID="Image1" runat="server" Height="4px" ImageUrl="~/images/transparent.png" />
                    </div>
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
            <tr>
                <td>
                    <b>Invoice Template (*):</b>
                </td>
                <td>
                    <div>
                        <ajaxToolkit:ComboBox ID="ComboBoxInvTpl" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                        <asp:CustomValidator ID="CustomValidatorInvTpl" runat="server" ForeColor="Red" 
                            ErrorMessage="!" onservervalidate="CustomValidatorInvTpl_ServerValidate"></asp:CustomValidator>
                    </div>
                    <div>
                        <asp:Image ID="Image2" runat="server" Height="4px" ImageUrl="~/images/transparent.png" />
                    </div>
                </td>
                <td>
                    <b>Statement Template (*):</b>
                </td>
                <td>
                    <div>
                        <ajaxToolkit:ComboBox ID="ComboBoxStmtTpl" runat="server" AutoPostBack="False" DropDownStyle="DropDownList"
                            AutoCompleteMode="SuggestAppend" CaseSensitive="False" ItemInsertLocation="Append"
                            Width="150px">
                        </ajaxToolkit:ComboBox>
                        <asp:CustomValidator ID="CustomValidatorStmtTpl" runat="server" ForeColor="Red" 
                            ErrorMessage="!" onservervalidate="CustomValidatorStmtTpl_ServerValidate"></asp:CustomValidator>
                    </div>
                    <div>
                        <asp:Image ID="Image3" runat="server" Height="4px" ImageUrl="~/images/transparent.png" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Agm Date:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxAgmDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarDue" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxAgmDate" />
                </td>
                <td>
                    <b>Agm Time:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxAgmTime" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Committee Date:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxCommitteeDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender1" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxCommitteeDate" />
                </td>
                <td>
                    <b>Committee Time:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxCommitteeTime" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Egm Date:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxEgmDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender2" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxEgmDate" />
                </td>
                <td>
                    <b>Egm Time:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxEgmTime" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Begin Date (*):</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxBeginDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender3" CssClass="sappcalendar"
                        Format="dd/MM/yyyy" TargetControlID="TextBoxBeginDate" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!"
                        ForeColor="Red" ControlToValidate="TextBoxBeginDate"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <b>Interest Close date:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxClosePeriodDate" runat="server" ClientIDMode="Static" Enabled="False"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <b>Egm Time:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBox2" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
                <td>
                    <b>Close Date :</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxCloseDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Notes:</b>
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" Width="500px"
                        ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr style="display: none;">
                <td>
                    <b>Inactive:</b>
                </td>
                <td>
                    <input type="checkbox" disabled="disabled" id="CheckBoxInactive" runat="server" />
                </td>
                <td>
                    <b>Inactive Date:</b>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxInactiveDate" runat="server" ReadOnly="true"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Close Date</div>
            <div>
                <asp:ImageButton ID="ImageButtonSave0" runat="server" ImageUrl="Images/save.gif"
                    OnClientClick="CloseDateClick();return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Interest Close Date</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/save.gif" OnClientClick="PCloseDateClick();return false;"/>
            </div>
        </div>
    </div>
    <script>
        function getQueryString(name) {
            if (location.href.indexOf("?") == -1 || location.href.indexOf(name + '=') == -1) {
                return '';
            }
            var queryString = location.href.substring(location.href.indexOf("?") + 1);
            var parameters = queryString.split("&");
            var pos, paraName, paraValue;
            for (var i = 0; i < parameters.length; i++) {
                pos = parameters[i].indexOf('=');
                if (pos == -1) { continue; }
                paraName = parameters[i].substring(0, pos);
                paraValue = parameters[i].substring(pos + 1);
                if (paraName == name) {
                    return unescape(paraValue.replace(/\+/g, " "));
                }
            }
            return '';
        };
        function CloseDateClick() {
            var url = "bodyCloseDate.aspx?bid=" + getQueryString("bodycorpid");
            vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 500px; dialogWidth: 650px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
            if (vReturnValue == "refresh") {
                __doPostBack('__Page', 'Refresh|0');
            }

        }
        function PCloseDateClick() {
            var url = "bodyInterestCloseDate.aspx?bid=" + getQueryString("bodycorpid");
            vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 500px; dialogWidth: 650px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
            if (vReturnValue == "refresh") {
                __doPostBack('__Page', 'Refresh|0');
            }

        }
    </script>
</asp:Content>
