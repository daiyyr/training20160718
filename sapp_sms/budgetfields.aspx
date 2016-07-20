<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="budgetfields.aspx.cs" Inherits="sapp_sms.budgetfields" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Budget Fields</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.11.datepicker.min.js" type="text/javascript"></script>
    <script src="scripts/budgetfields.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div id="content_left">
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="~/images/delete.gif" PostBackUrl="~/budgetfields.aspx"
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
              <cc1:jqGridAdv runat="server" ID="jqGridBudgetFields" colNames="['ID','Name(*)','GL Code(*)','Scale','Start','End','Order']"
             colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                  { name: 'Name', index: 'Name', width: 100, editable:true, editoptions: { 'maxlength': 100 }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'GlCode', index: 'GlCode', width: 50, editable:true, edittype:'select', editoptions:{dataUrl:'budgetfields.aspx/BindGlCodeSelector'},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Scale', index: 'Scale', width: 50, editable:true, edittype:'select', editoptions:{dataUrl:'budgetfields.aspx/BindScaleSelector'},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Start', index: 'Start', width: 50, editable:true,editoptions:{size:20,dataInit:function(el){ $(el).datepicker({dateFormat:'yy-mm-dd'}); }}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'End', index: 'End', width: 50, editable:true,editoptions:{size:20,dataInit:function(el){ $(el).datepicker({dateFormat:'yy-mm-dd'}); }}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Order', index: 'Order', width: 100, editable:true, align: 'left', search: false, hidden:true}
              ]"
             rowNum="25"
             rowList="[5, 10, 25, 50, 100]"
             sortname="Order"
             sortorder="asc"
             viewrecords="true"
             width="700"
             height="500"
             url="budgetfields.aspx/DataGridDataBind"
             hasID="true"
             idName="bodycorpid"
             editurl="budgetfields.aspx/SaveDataFromGrid"
             inlineNav="true"
             />
        </div>
            
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">Create Chart Base</div>
            <div>
                <asp:ImageButton ID="ImageButtonChartBase"
                    runat="server" ImageUrl="~/images/account.gif" 
                    onclick="ImageButtonChartBase_Click" />
            </div>
        </div>
    </div>
</asp:Content>
