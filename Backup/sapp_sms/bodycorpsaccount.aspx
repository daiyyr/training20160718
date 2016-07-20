<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="bodycorpsaccount.aspx.cs" Inherits="sapp_sms.bodycorpsaccount" %>

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
    <script src="Scripts/bodycorps.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
    </div>
    <div id="content_middle">
        <div>
            <cc1:jqGridAdv runat="server" ID="jqgrid" colNames="['Num', 'BcNum', 'UnitNum', 'Password']"
                colModel="[		
                    { name: 'Num', index: 'Num', width: 45, align: 'left', sorttype: 'int', hidden:true},
                     { name: 'BcNum', index: 'BcNum', width: 200, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['cn', 'nc']}, editable:true},
                     { name: 'UnitNum', index: 'UnitNum', width: 105, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                     { name: 'Password', index: 'Password', width: 105, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                     ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" viewrecords="true" width="700"
                height="500" url="bodycorpsaccount.aspx/DataBind" hasID="false" />
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Report</div>
            <div>
                <asp:ImageButton ID="ImageButtonLevies" ImageUrl="~/images/levies.gif" 
                    runat="server" onclick="ImageButtonLevies_Click" />
            </div>
        </div>
    </div>
</asp:Content>
