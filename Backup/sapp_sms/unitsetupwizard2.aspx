<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="unitsetupwizard2.aspx.cs" Inherits="sapp_sms.unitsetupwizard2" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Setup Wizard Step 1</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/unitsetupwizard2.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
        <div class="button-title">Save</div>
            <div>
                <asp:ImageButton ID="ImageButtonSave"
                    runat="server" ImageUrl="Images/save.gif" 
                    onclick="ImageButtonSave_Click" />
            </div>
        </div>
        <div  class="button">
        <div class="button-title">Back</div>
        <div>
            <asp:ImageButton ID="ImageButtonClose" 
                runat="server" ImageUrl="Images/goback.gif" CausesValidation="false"  OnClientClick="history.back(); return false;" />
        </div>
        </div>
    </div>
    <div id="content_middle">
        <div>
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridUnitMaster" colNames="['ID','Unit Num','Type','Description','Proprietor','Area Sqm','Area Type','OI','UI']"
             colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                  { name: 'Code', index: 'Code', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Type', index: 'Type', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Description', index: 'Description', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Proprietor', index: 'Proprietor', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaSqm', index: 'AreaSqm', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaType', index: 'AreaType', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'OI', index: 'OI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'UI', index: 'UI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
              ]"
             rowNum="25"
             rowList="[5, 10, 25, 50, 100]"
             sortname="Code"
             sortorder="asc"
             viewrecords="true"
             width="800"
             height="500"
             url="unitsetupwizard2.aspx/jqGridUnitMasterDataBind"
             hasID="true"
             idName="propertyid"
             subGrid="true"
             sColNames="['ID','Unit Num','Type','Description','Proprietor','Area Sqm','Area Type','OI','UI']"
             sColModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                  { name: 'Code', index: 'Code', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Type', index: 'Type', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Description', index: 'Description', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Proprietor', index: 'Proprietor', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaSqm', index: 'AreaSqm', width: 50,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaType', index: 'AreaType', width: 50,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'OI', index: 'OI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'UI', index: 'UI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                ]"
            sSortName="Code"
            sSortOrder="asc"
            sUrl="unitsetupwizard2.aspx/jqGridUnitMasterFillSubGrid"
            sWidth="700"
             />
             
        </div>
        <div style="text-align:right; background-color:White">
            <b>Total</b>&nbsp;&nbsp;&nbsp; 
            <b>OI:</b>&nbsp;<asp:Label ID="LabelTotalOI" runat="server" ForeColor="Blue" ></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <b>UI:</b>&nbsp;<asp:Label ID="LabelTotalUI" runat="server" ForeColor="Blue"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </div>
            
    </div>
    <div id="content_right">
    </div>
</asp:Content>
