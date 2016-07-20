<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="ownerships.aspx.cs" Inherits="sapp_sms.ownerships" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Ownerships</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.datepicker.min.js" type="text/javascript"></script>
    <script src="scripts/ownerships.js" type="text/javascript"></script>
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
            <cc1:jqGridAdv runat="server" ID="jqGridOwnerships" colNames="['ID','Debtor(*)' ,'Start(*)', 'End(*)', 'Notes']"
                colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: false, hidden:true},
                       
             { name: 'Debtor', index: 'Debtor', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'ownerships.aspx/BindDebtorSelector'}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
             { name: 'Start', index: 'Start', width: 45, editable:true, editoptions:{size:20,dataInit:function(el){ $(el).datepicker({dateFormat:'dd-mm-yy'}); }}, editrules:{custom:true, custom_func:NotNull}, align: 'left', formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}, search: true,  searchoptions: { sopt: ['cn', 'nc']}},
             { name: 'End', index: 'End', width: 45, editable:true, editoptions:{size:20,dataInit:function(el){ $(el).datepicker({dateFormat:'dd-mm-yy'}); }}, editrules:{custom:true, custom_func:NotNull}, align: 'left', formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}, search: true,  searchoptions: { sopt: ['cn', 'nc']}},
             { name: 'Notes', index: 'Notes', width: 200, editable:true, edittype:'textarea', editoptions: { 'maxlength': 250 }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
             ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                width="700" height="500" url="ownerships.aspx/jqGridOwnershipsDataBind" hasID="false" />
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Details</div>
            <div>
                <asp:ImageButton ID="ImageButtonDetails" runat="server" ImageUrl="Images/detail.gif"
                    OnClientClick="ImageButton_ClientClick()" />
                <script type="text/javascript">

                    function ImageButton_ClientClick() {

                        var grid = jQuery("#" + GetClientId("jqGridOwnerships") + "_datagrid1");
                        var rowKey = grid.getGridParam("selrow");

                        if (rowKey) {
                            var ID = grid.getCell(rowKey, 'ID');
                            window.open("debtormasterdetails.aspx?debtorid=" + ID);
                            return false;
                        }
                        else {
                            alert("Please select a row first!");
                            return false;
                        }

                        return false;

                    }
                </script>
            </div>
        </div>
    </div>
</asp:Content>
