<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="authorities.aspx.cs" Inherits="sapp_sms.authorities" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Authorities</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/authorities.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div id="content_left">
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="~/images/delete.gif" PostBackUrl="~/authorities.aspx"
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
            <cc1:jqGridAdv runat="server" ID="jqGridAuthorities" colNames="['ID','Code(*)' ,'Name(*)','Description']"
             colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
             { name: 'Code', index: 'Code', width: 100, editable:true, editoptions: { 'maxlength': 20 },editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
             { name: 'Name', index: 'Name', width: 100, editable:true, editoptions: { 'maxlength': 100 },editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
             { name: 'Description', index: 'Description', width: 100, editable:true, editoptions: { 'maxlength': 100 }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}]"
             rowNum="25"
             rowList="[5, 10, 25, 50, 100]"
             sortname="ID"
             sortorder="asc"
             viewrecords="true"
             width="700"
             height="500"
             url="authorities.aspx/DataGridAuthoritiesDataBind"
             hasID="false"
             editurl="authorities.aspx/SaveDataFromGrid"
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

