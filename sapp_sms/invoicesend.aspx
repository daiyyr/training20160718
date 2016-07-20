<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="invoicesend.aspx.cs" Inherits="sapp_sms.invoicesend" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Invoice Sending Wizard</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />

    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/invoicesend.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div id="content_left">
        <div class="button">
            <div class="button-title">Create Attachment</div>
            <div>
                <asp:ImageButton ID="ImageButtonNewReport" runat="server" ImageUrl="Images/save.gif" PostBackUrl="~/invoicesend.aspx" CausesValidation="false" OnClientClick=" return ImageButtonNewReport_ClientClick();" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">Send Email</div>
            <div>
                <asp:ImageButton ID="ImageButtonSendEmail" runat="server" ImageUrl="Images/detail.gif" PostBackUrl="~/invoicemails.aspx" OnClientClick="return ImageButtonSendEmail_ClientClick();" />
            </div>
        </div>
        <div  class="button">
            <div class="button-title">Cancel</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif" CausesValidation="false"  OnClientClick="history.back(); return false;" />
            </div>
        </div>
    </div>

    <div id="content_middle">
         

    <table class="details">
        <tr>
            <td width="140px"><b>Bodycorp:</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxBodycorp" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td><b>Date Between:</b></td>
            <td>
                <asp:TextBox ID="TextBoxStart" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server" 
                    ID="CalendarExtender1" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxStart">
                 </ajaxToolkit:CalendarExtender>  
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxStart"></asp:RequiredFieldValidator>     
            </td>
            <td><b> To:</b></td>
            <td>
                <asp:TextBox ID="TextBoxEnd" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server" 
                    ID="CalendarExtender2" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxEnd">
                 </ajaxToolkit:CalendarExtender>  
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxEnd"></asp:RequiredFieldValidator>     
            </td>
        </tr>
    </table>
    <br/>
    <table class="details">
        <tr>
            <td colspan="4"><b>Email</b></td>
        </tr>
        <tr>
            <td width="140px">
                <b>Send Statement:</b>
            </td>
            <td>
                 <asp:CheckBox ID="CheckBoxStmt" ClientIDMode="Static" runat="server" />
            </td>
            <td><b>Statement Date:</b></td>
            <td>
                <asp:TextBox ID="TextBoxStmtStartDate" ClientIDMode="Static" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender3" CssClass="sappcalendar" Format="dd/MM/yyyy" TargetControlID="TextBoxStmtStartDate"></ajaxToolkit:CalendarExtender>
                &nbsp;<b> To:</b>&nbsp;
                <asp:TextBox ID="TextBoxStmtEndDate" ClientIDMode="Static" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender4" CssClass="sappcalendar" Format="dd/MM/yyyy" TargetControlID="TextBoxStmtEndDate"></ajaxToolkit:CalendarExtender>
            </td>
        </tr>
        <tr>
            <td><b>Notes:</b></td>
            <td colspan="3">
                <asp:CheckBox ID="CheckBoxNotes" ClientIDMode="Static" runat="server" />&nbsp;
                <asp:FileUpload ID="FileUploadNotes" ClientIDMode="Static" runat="server" Width="450px" accept="application//pdf" />
            </td>
        </tr>
        <tr>
            <td><b>Emial Subject:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxSubject" runat="server" ClientIDMode="Static" Width="580px" MaxLength="250"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td><b>Emial Body:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxBody" runat="server" ClientIDMode="Static" Width="580px" Height="70px" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br/>
    <table class="details">
        <tr>
            <td colspan="4"><b>Invoice List:</b></td>
        </tr>
        <tr>
            <td colspan="4">
                <div>
                    <img src="Images/dot.gif" height="4px" />
                    <cc1:jqGridAdv runat="server" ID="jqGridInvoice" colNames="['ID','Inv Num','Debtor','Date','Due', 'Total', 'Invoice', 'Statement', 'Notes']"
                        colModel="[
                            { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                            { name: 'Num', index: 'Num', width: 50, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                            { name: 'Debtor', index: 'Debtor', width: 100, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                            { name: 'Date', index: 'Date', width: 50, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                            { name: 'Due', index: 'Due', width: 50, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                            { name: 'Total', index: 'Total', width: 50, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                            { name: 'Invoice', Invoice: 'Due', width: 50, editable:false, align: 'center', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                            { name: 'Statement', Statement: 'Due', width: 50, editable:false, align: 'center', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                            { name: 'Notes', index: 'Notes', width: 50, editable:false, align: 'center', search: true,  searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}}
                        ]"
                        rowNum="10000"
                        rowList="[]"
                        sortname="ID"
                        sortorder="asc"
                        viewrecords="true"
                        multiselect="true"
                        width="700"
                        height="300"
                        url="invoicesend.aspx/jqGridInvoiceDataBind"
                        hasID="false"
                        inlineNav="true"
                        />

                    <asp:HiddenField ID="HiddenSelectedIdxs" runat="server" ClientIDMode="Static" />
                </div>
            </td>
        </tr>
    </table>
    </div>
    <div id="content_right">
        <div class="button">
        <div class="button-title">Load</div>
        <div>
            <asp:ImageButton ID="ImageButtonLoad"
            runat="server" ImageUrl="Images/save.gif" onclick="ImageButtonLoad_Click"/>
        </div>
    </div>
    </div>
</asp:Content>
