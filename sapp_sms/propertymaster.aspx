<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="propertymaster.aspx.cs" Inherits="sapp_sms.propertymaster" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Property Master</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/propertymaster.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">Add</div>
            <div>
                <asp:ImageButton ID="ImageButtonAdd"
                    runat="server" ImageUrl="Images/new.gif" onclick="ImageButtonAdd_Click"  />
            </div>
        </div>
        <div  class="button">
        <div class="button-title">Details</div>
        <div>
            <asp:ImageButton ID="ImageButtonDetails"
                runat="server" ImageUrl="Images/detail.gif" PostBackUrl="~/propertymaster.aspx"
                OnClientClick="return ImageButtonDetails_ClientClick()"
                />
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Edit</div>
        <div>
            <asp:ImageButton ID="ImageButtonEdit"
                runat="server" ImageUrl="Images/edit.gif" PostBackUrl="~/propertymaster.aspx"
                OnClientClick="return ImageButtonEdit_ClientClick()"/>
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="Images/delete.gif" PostBackUrl="~/propertymaster.aspx"
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
                <cc1:jqGridAdv runat="server" ID="jqGridProperty" colNames="['ID', 'Code', 'Type', 'Name']"
                 colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                 { name: 'Code', index: 'Code', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Type', index: 'Type', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Name', index: 'Name', width: 150, align: 'left', search: false}
                 ]"
                 rowNum="25"
                 rowList="[5, 10, 25, 50, 100]"
                 sortname="ID"
                 sortorder="asc"
                 viewrecords="true"
                 width="700"
                 height="500"
                 url="propertymaster.aspx/DataGridDataBind"
                 hasID="true"
                 idName="menuid"
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
