<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bodycorpdetails.aspx.cs" Inherits="sapp_sms.bodycorpdetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - BodyCorp Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/bodycorpdetails.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Super Search</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/search.gif" 
                    OnClick="ImageButton2_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="~/images/edit.gif"
                    OnClick="ImageButtonEdit_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                    OnClick="ImageButtonDelete_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonHome" runat="server" OnClientClick="history.back(); return false;"
                    ImageUrl="~/images/close.gif" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">General</a></li>
                <li><a href="#tabs-2">Comms</a></li>
                <li><a href="#tabs-3">Logs</a></li>
                <li><a href="#tabs-4">Files</a></li>
            </ul>
            <div id="tabs-1">
                <table class="details">
                    <tr>
                        <td>
                            <b>Bodycorp:<asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td colspan="3">
                            <b>
                                <asp:Label ID="LabelBodycorpID" runat="server" Text="ID" Visible="False" CssClass="sapplabel"></asp:Label></b>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>BC Num:<asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td>
                            <asp:Label ID="LabelCode" runat="server" Text="LabelCode" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                            <b>Name:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelName" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>GST:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelGST" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                            <b>GST Registered:</b>
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBoxGST" runat="server" Enabled=false />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
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
                            <asp:CheckBox ID="CheckBoxDiscount" runat="server" Enabled=false/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Bank Account :</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelAccount" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Agm Date:<asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td>
                            <asp:Label ID="LabelAgmDate" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                            <b>Agm Time:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelAgmTime" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Committee Date:<asp:Image ID="Image13" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td>
                            <asp:Label ID="LabelCommitteeDate" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                            <b>Committee Time:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelCommitteeTime" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image14" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Egm Date:<asp:Image ID="Image21" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td>
                            <asp:Label ID="LabelEgmDate" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                            <b>Egm Time:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelEgmTime" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image22" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Begin Date:<asp:Image ID="Image15" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td>
                            <asp:Label ID="LabelBeginDate" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                            <b>Interest Close period</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelClosePeriodDate" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <b>Close Date:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelCloseDate" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image16" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td>
                            <b>Inactive:<asp:Image ID="Image17" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td>
                            <asp:Label ID="LabelInactive" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                        <td>
                            <b>Inactive Date:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelInactiveDate" runat="server" CssClass="sapplabel"></asp:Label>
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td colspan="4">
                            <asp:Image ID="Image18" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Notes:<asp:Image ID="Image19" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                            </b>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" Width="500px"
                                ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image20" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabs-2">
                <div>
                    <cc1:jqGridAdv runat="server" ID="jqGridComms" colNames="['ID', 'Type(*)', 'Details(*)', 'Primary', 'Order']"
                        colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                    { name: 'Type', index: 'Type', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'bodycorpdetails.aspx/DataGridCommsTypeSelect'}, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Details', index: 'Details', width: 200, editable:true,editoptions: { 'maxlength': 100 } ,editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: false},
                    { name: 'Primary', index: 'Primary', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'Yes:No'}, align: 'left', search: false},
                    { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden: true}
                    ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Order" sortorder="asc" viewrecords="true"
                        width="700" height="500" url="bodycorpdetails.aspx/DataGridCommsDataBind" hasID="true"
                        idName="bodycorpid" inlineNav="true" editurl="bodycorpdetails.aspx/DataGridCommsSave"
                        contentPlaceHolder="ContentPlaceHolder1" />
                </div>
                <div align="left">
                    <asp:Button ID="ButtonDeleteComm" runat="server" Text="Delete" OnClientClick="return ButtonDeleteComm_ClientClick()" />
                </div>
            </div>
            <div id="tabs-3">
                <div>
                    <cc1:jqGridAdv runat="server" ID="jqGridLogs" colNames="['ID', 'User', 'Action', 'DateTime', 'Details']"
                        colModel="[{ name: 'ID', index: 'ID', width: 45,editable:false, align: 'left', sorttype: 'int', search: false, hidden:true},
                    { name: 'User', index: 'User', width: 45, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Action', index: 'Action', width: 45, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'DateTime', index: 'DateTime', width: 45, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Details', index: 'Details', width: 200, align: 'left', search: true, searchoptions: { sopt: [ 'cn', 'nc']}}
                    ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                        width="700" height="500" url="bodycorpdetails.aspx/jqGridLogsDataBind" hasID="false" />
                </div>
            </div>
            <div id="tabs-4">
                <div>
                    <cc1:jqGridAdv runat="server" ID="jqGridFiles" colNames="['ID', 'File Name', 'Date Created', 'Type', 'Size']"
                        colModel="[{ name: 'ID', index: 'ID', width: 45, align: 'left', sorttype: 'int', hidden:true},
                    { name: 'FileName', index: 'FileName', width: 200, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Date', index: 'Date', width: 45, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Type', index: 'Type', width: 45, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Size', index: 'Size', width: 45, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                        width="700" height="500" url="bodycorpdetails.aspx/jqGridFilesDataBind" hasID="false" />
                </div>
                <div style="text-align: left;">
                    <asp:FileUpload ID="FileUpload1" runat="server" BackColor="White" />
                    <asp:Button ID="ButtonUpload" runat="server" Text="Upload" OnClick="ButtonUpload_Click" />
                    <asp:Button ID="ButtonDownload" runat="server" Text="Download" PostBackUrl="~/bodycorpdetails.aspx"
                        OnClientClick="return ButtonDownload_ClientClick()" />
                    <asp:Button ID="ButtonDelete" runat="server" Text="Delete" PostBackUrl="~/bodycorpdetails.aspx"
                        OnClientClick="return ButtonDelete_ClientClick()" />
                </div>
                <div style="text-align: left;">
                    <asp:Button ID="ButtonHome" runat="server" Text="Home" OnClick="ButtonHome_Click" />
                    <asp:Button ID="ButtonEnter" runat="server" Text="Enter" PostBackUrl="~/bodycorpdetails.aspx"
                        OnClientClick="return ButtonEnter_ClientClick()" />
                    <asp:Button ID="ButtonReturn" runat="server" Text="Return" OnClick="ButtonReturn_Click" />
                </div>
            </div>
        </div>
        <asp:Panel ID="PanelSelector" CssClass="PopupCSS" runat="server">
            <div>
                <table class="modaltable">
                    <tr>
                        <td>
                            Start Date:
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxStartDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender runat="server" CssClass="sappcalendar" ID="CalendarStartDate"
                                Format="dd/MM/yyyy" TargetControlID="TextBoxStartDate" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Image ID="Image10" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="ButtonOK" runat="server" Text="OK" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:HiddenField ID="HiddenBCID" ClientIDMode="Static" runat="server" />
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="MPE" runat="server" TargetControlID="ImageButtonBudget"
            PopupControlID="PanelSelector" BackgroundCssClass="modalBackground" DropShadow="true"
            OkControlID="ButtonOK" OnOkScript="ButtonOKClick()" CancelControlID="ButtonCancel">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="PanelActivitySelector" CssClass="PopupCSS" runat="server">
            <div>
                <table class="modaltable">
                    <tr>
                        <td>
                            Start Date:
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxActivityStart" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderActivityStart" CssClass="sappcalendar"
                                Format="dd/MM/yyyy" TargetControlID="TextBoxActivityStart" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            End Date:
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxActivityEnd" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderActivityEnd" CssClass="sappcalendar"
                                Format="dd/MM/yyyy" TargetControlID="TextBoxActivityEnd" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="ButtonActivityOK" runat="server" Text="OK" />
                            <asp:Button ID="ButtonActivityCancel" runat="server" Text="Cancel" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="MPE2" runat="server" TargetControlID="ImageButtonActivity"
            PopupControlID="PanelActivitySelector" BackgroundCssClass="modalBackground" DropShadow="true"
            OkControlID="ButtonActivityOK" OnOkScript="ButtonActivityOKClick()" CancelControlID="ButtonActivityCancel">
        </ajaxToolkit:ModalPopupExtender>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Activity</div>
            <div>
                <asp:ImageButton ID="ImageButtonActivity" ImageUrl="~/images/activities.gif" runat="server" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Budget</div>
            <div>
                <asp:ImageButton ID="ImageButtonBudget" ImageUrl="~/images/budget.gif" runat="server"
                    OnClick="ImageButtonBudget_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Levies</div>
            <div>
                <asp:ImageButton ID="ImageButtonLevies" ImageUrl="~/images/levies.gif" runat="server"
                    OnClientClick="ImageButtonLevies_ClientClick(); return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Managers</div>
            <div>
                <asp:ImageButton ID="ImageButtonManager" ImageUrl="~/images/manager.gif" runat="server"
                    OnClick="ImageButtonManager_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Property</div>
            <div>
                <asp:ImageButton ID="ImageButtonProperty" ImageUrl="~/images/property.gif" runat="server"
                    OnClick="ImageButtonProperty_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Proprietors</div>
            <div>
                <asp:ImageButton ID="ImageButtonDebtor" ImageUrl="~/images/proprietor.gif" runat="server"
                    OnClick="ImageButtonDebtor_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Financial Reports</div>
            <div>
                <asp:ImageButton ID="ImageButtonReports" ImageUrl="~/images/report.gif" runat="server"
                    OnClientClick="ImageButtonReports_ClientClick(); return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Unit Plan</div>
            <div>
                <asp:ImageButton ID="ImageButtonUnitPlan" runat="server" ImageUrl="~/images/unitplan.gif"
                    OnClick="ImageButtonUnitplan_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Interest</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/dollar.gif"
                    OnClick="ImageButton1_Click" />
            </div>
        </div>
        <%--   <div class="button">
            <div class="button-title">
                Maintenance</div>
            <div>
                <asp:ImageButton ID="ImageButtonMaintenance" runat="server" ImageUrl="Images/Maintenance.gif"
                    OnClick="ImageButtonMaintenance_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Valuations</div>
            <div>
                <asp:ImageButton ID="ImageButtonValuations" runat="server" ImageUrl="Images/valuation.gif"
                    OnClick="ImageButtonValuations_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Titles</div>
            <div>
                <asp:ImageButton ID="ImageButtonTitles" runat="server" ImageUrl="Images/Title.gif"
                    OnClick="ImageButtonTitles_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Mortgages</div>
            <div>
                <asp:ImageButton ID="ImageButtonMortgages" runat="server" ImageUrl="Images/mortgage.gif"
                    OnClick="ImageButtonMortgages_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Contractor</div>
            <div>
                <asp:ImageButton ID="ImageButtonContractor" runat="server" ImageUrl="Images/contractor.gif"
                    OnClick="ImageButtonContractor_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Insurance</div>
            <div>
                <asp:ImageButton ID="ImageButtonInsurance" runat="server" ImageUrl="Images/insurance.gif"
                    OnClick="ImageButtonInsurance_Click" />
            </div>
        </div>--%>
        <div class="button">
            <div class="button-title">
                Portal</div>
            <div>
                <asp:ImageButton ID="AccountB" runat="server" ImageUrl="Images/portal.gif" OnClick="AccountB_Click" />
            </div>
        </div>
    </div>
</asp:Content>
