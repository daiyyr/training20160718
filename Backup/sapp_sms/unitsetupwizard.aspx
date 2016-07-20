<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="unitsetupwizard.aspx.cs" Inherits="sapp_sms.unitsetupwizard" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Setup Wizard Step 1</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="scripts/unitsetupwizard.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div id="content_left">
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="~/images/delete.gif" PostBackUrl="~/unitsetupwizard.aspx"
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
            <cc1:jqGridAdv runat="server" ID="jqGridUnitPlan" colNames="['ID','Unit Num(*)', 'Type(*)','Principal','Description','Proprietor','Sqm','Area Type','OI','UI','Special','Committee']"
             colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', search: false, hidden:true},
                  { name: 'Code', index: 'Code', width: 100, editable:true, editrules:{custom:true, custom_func:ValidateCode},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Type', index: 'Type', width: 120, editable:true, edittype:'select', editoptions:{dataUrl:'unitsetupwizard.aspx/BindUnitTypeSelector'},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Principle', index: 'Principle', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'unitsetupwizard.aspx/BindPrincipalSelector'}, editrules:{custom:true, custom_func:ValidatePrinciple},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Description', index: 'Description', editable:true,  width: 150, editoptions: { dataEvents: [{type: 'focusout', fn: function(e) {ReplaceDQuote(this);}}]}, editrules:{custom:true, custom_func:StrNoDQuote},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Proprietor', index: 'Proprietor', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'unitsetupwizard.aspx/BindDebtorSelector'},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaSqm', index: 'AreaSqm', editable:true, editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaType', index: 'AreaType', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'unitsetupwizard.aspx/BindAreaTypeSelector'},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'OI', index: 'OI', editable:true,editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'UI', index: 'UI', editable:true,editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'SpecialScale', index: 'SpecialScale', editable:true,editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Committee', index: 'Committee', editable:true, edittype:'checkbox', editoptions:{value:'true:false'}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
              ]"
             rowNum="25"
             rowList="[5, 10, 25, 50, 100]"
             sortname="Code"
             sortorder="asc"
             viewrecords="true"
             width="800"
             height="500"
             url="unitsetupwizard.aspx/UnitPlanDataBind"
             hasID="false"
             editurl="unitsetupwizard.aspx/UnitPlanSaveData"
             inlineNav="true"
             afterRowSave="ChangeTotal"
             />
        </div>
        <div style="text-align:right; background-color:White">
            <b>Total</b>&nbsp;&nbsp;&nbsp; 
            <b>OI:</b>&nbsp;<asp:Label ID="LabelTotalOI" runat="server" ForeColor="Blue" ></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <b>UI:</b>&nbsp;<asp:Label ID="LabelTotalUI" runat="server" ForeColor="Blue"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </div>
    </div>
    <div id="content_right">
        <div  class="button">
        <div class="button-title">Next Step</div>
        <div>
            <asp:ImageButton ID="ImageButtonNext"
                runat="server" ImageUrl="~/images/goright.gif" 
                onclick="ImageButtonNext_Click"  />
        </div>
        </div>
    </div>
</asp:Content>
