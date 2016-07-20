<%@ Page Title="" Language="C#" MasterPageFile="~/popup.Master" AutoEventWireup="true"
    CodeBehind="activityreciptdetail.aspx.cs" Inherits="sapp_sms.activityreciptdetail" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            height: 9px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="details" style="background-color: #FFFFFF">
        <tr>
            <td>
                Bodycorp:<asp:Label ID="bodycorpL" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                Type:<asp:Label ID="TypeL" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <cc1:jqGridAdv runat="server" ID="jqGridActivityUnit" colNames="['Num','Code', 'Name','Date','Ref','Description','Amount']"
                    colModel="[
                                      { name: 'Num', index: 'Num', width: 50,editable:false, align: 'left'},

                  { name: 'Code', index: 'Code', width: 50,editable:false, align: 'left', sorttype: 'int'},
                  { name: 'Name', index: 'Name', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Date', index: 'Date', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Ref', index: 'Ref', width: 200,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Description', index: 'Description', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
                  { name: 'Amount', index: 'Amount', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}, sortable:false},
              ]" rowNum="500" rowList="[100, 200, 500, 1000, 5000]" sortname="Date" sortorder="asc"
                    viewrecords="true" width="800" height="500" url="activityreciptdetail.aspx/DataGridDataBind"
                    hasID="false" footerrow="true" userDataOnFooter="true" />
            </td>
        </tr>
    </table>
</asp:Content>
