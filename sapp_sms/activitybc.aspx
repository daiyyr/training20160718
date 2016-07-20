<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="activitybc.aspx.cs" Inherits="sapp_sms.activitybc" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Bodycorp Activity</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/activity.js" type="text/javascript"></script>
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
        <div>
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridActivity" colNames="['Code', 'Name']"
                colModel="[
                  { name: 'chart_code', index: 'chart_code', width: 50,editable:false, align: 'left', sorttype: 'int'},
                  { name: 'chart_name', index: 'chart_name', width: 100}
             
              ]" rowNum="500" rowList="[100, 200, 500, 1000, 5000]" sortname="Date" sortorder="asc"
                viewrecords="true" width="800" height="500" url="activitybc.aspx/jqGridActivityDataBind"
                hasID="false" footerrow="true" userDataOnFooter="true" subGrid="true" sColNames="['Code',  'Date', 'Reference', 'Description','Debit', 'Credit']"
                sColModel="[
                  { name: 'chart_code', index: 'chart_code', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                  { name: 'date', index: 'date', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'reference', index: 'reference', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'description', index: 'description', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'debit', index: 'debit', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'credit', index: 'credit', width: 50,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                ]" sUrl="activitybc.aspx/FillSubGrid" sWidth="700" />
            <div style="background-color: White">
            </div>
        </div>
        <input id="Button1" type="button" value="button" />
        <%--        <script>
            $("#Button1").click(function () {
                alert("1");
                var grid = jQuery("#" + GetClientId("jqGridActivity") + "_datagrid1");
                grid.jqGrid('setGridParam', { grouping: true });
                grid.jqGrid('groupingRemove', true);
                grid.jqGrid('groupingGroupBy', ['Date']);
//                grid.jqGrid('groupingGroupBy', "Date");
                grid.trigger('reloadGrid');
                alert("2");
            })
            $(document).ready(function () {


            })
        </script>--%>
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
