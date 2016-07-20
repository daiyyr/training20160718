<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="contactmasters.aspx.cs" Inherits="sapp_sms.contactmasters" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<%@ Register src="jqGridCC.ascx" tagname="jqGridCC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Contact Master</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/contactmasters.js" type="text/javascript"></script>
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
            <b>Contact List</b>
            <cc1:jqGridAdv runat="server" ID="jqGridCC1" colNames="['ID','Type', 'Name(*)','Notes']"
                colModel="[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}, hidden:true},
                { name: 'Type', index: 'Type', width: 100,editable:true, edittype:'select', editoptions:{dataUrl:'contactmasters.aspx/DataGridContactTypeSelect'}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                { name: 'Name', index: 'Name', width: 100,editable:true,editoptions: { 'maxlength': 100 },editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                { name: 'Notes', index: 'Notes', width: 100,editable:true,editoptions: { 'maxlength': 250 }, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                ]"
                rowNum="25"
                rowList="[5, 10, 25, 50, 100]"
                sortname="ID"
                sortorder="asc"
                viewrecords="true"
                width="700"
                height="200"
                url="contactmasters.aspx/DataGridDataBind"
                hasID="false"
                editurl="contactmasters.aspx/SaveDataFromGrid"
                inlineNav="true"

                 DetailPageUrl="contactmasters.aspx/DataGridSubDataBind"
                 dtitle="Communication Details"
                 dcolNames="['ID','Type','Details(*)','Primary','Order']"
                 dcolModel="[
                    { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                    { name: 'Type', index: 'Type', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'contactmasters.aspx/DataGridCommsTypeSelect'}, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Details', index: 'Details', width: 200, editable:true,editoptions: { 'maxlength': 100 },editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: false},
                    { name: 'Primary', index: 'Primary', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'1:0'},  align: 'left', search: false},
                    { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden: true}
                 ]"
                 drowNum="25"
                 drowList="[5, 10, 25, 50, 100]"
                 dsortName="ID"
                 dsortOrder="asc"
                 dwidth="700"
                 dheight="200"
                 dmultiSelect="false"
                 deditUrl="contactmasters.aspx/SaveDataFromCommGrid"
            />
            <div align="left">
                <asp:Button ID="ButtonDeleteComm" runat="server" Text="Delete" OnClientClick="return ButtonDeleteComm_ClientClick()" />
            </div>
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
