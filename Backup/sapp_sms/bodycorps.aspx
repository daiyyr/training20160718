<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="bodycorps.aspx.cs" Inherits="sapp_sms.bodycorps" %>
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
                runat="server" ImageUrl="Images/detail.gif" PostBackUrl="~/bodycorps.aspx"
                OnClientClick="return ImageButtonDetails_ClientClick()"
                />
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Edit</div>
        <div>
            <asp:ImageButton ID="ImageButtonEdit"
                runat="server" ImageUrl="Images/edit.gif" PostBackUrl="~/bodycorps.aspx"
                OnClientClick="return ImageButtonEdit_ClientClick()"/>
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="Images/delete.gif" PostBackUrl="~/bodycorps.aspx"
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
                <cc1:jqGridAdv runat="server" ID="jqGridBodycorps" colNames="['ID', 'BC Num', 'Name', 'Address']"
                 colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: false, hidden:true},
                 { name: 'Code', index: 'Code', width: 50, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Name', index: 'Name', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Address', index: 'Address', width: 200, align: 'left', search: true, searchoptions: { sopt: [ 'cn', 'nc']}}
                 ]"
                 rowNum="50"
                 rowList="[5, 10, 25, 50, 100]"
                 sortname="ID"
                 sortorder="asc"
                 viewrecords="true"
                 width="800"
                 height="700"
                 url="bodycorps.aspx/DataGridBodycorpsDataBind"
                 hasID="true"
                 idName="menuid"
                 />
            </div>      
    </div>
    <div id="content_right">
        
    </div>
</asp:Content>

