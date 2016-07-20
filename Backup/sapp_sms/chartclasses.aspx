<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="chartclasses.aspx.cs" Inherits="sapp_sms.chartclasses" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<%@ Register src="jqGridCC.ascx" tagname="jqGridCC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - ChartClasses</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/chartclasses.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div id="content_left">
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="~/images/delete.gif" PostBackUrl="~/chartclasses.aspx"
                OnClientClick="return ImageButtonDelete_ClientClick()"/>
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Close</div>
        <div>
            <asp:ImageButton ID="ImageButtonClose" 
                runat="server" ImageUrl="Images/close.gif" CausesValidation="false"  OnClientClick="history.back(); return false;" />
        </div>
        </div>
    </div>
    <div id="content_middle">
        <div>
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridChartClasses" colNames="['ID','Name(*)','BalanceSheet(*)', 'Credit']"
             colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                  { name: 'Name', index: 'Name', width: 100, editable:true, editoptions: { 'maxlength': 100 }, editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'BalanceSheet', index: 'BalanceSheet', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'true:false'}, align: 'left', search: false},
                  { name: 'Credit', index: 'Credit', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'true:false'}, align: 'left', search: false}
              ]"
             rowNum="25"
             rowList="[5, 10, 25, 50, 100]"
             sortname="ID"
             sortorder="asc"
             viewrecords="true"
             width="700"
             height="500"
             url="chartclasses.aspx/DataGridChartClassesDataBind"
             hasID="false"
             editurl="chartclasses.aspx/SaveDataFromGrid"
             inlineNav="true"
             />
            
        </div>
            
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title"></div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>

