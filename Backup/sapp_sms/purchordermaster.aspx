<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="purchordermaster.aspx.cs" Inherits="sapp_sms.purchordermaster" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Purch Order Master</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/purchordermaster.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div id="content_left">
        <div class="button">
        <div class="button-title">Allocated</div>
        <div>
            <ajaxToolkit:ComboBox ID="ComboBoxAllocated" runat="server"
                AutoPostBack="True"
                DropDownStyle="DropDownList"
                AutoCompleteMode="SuggestAppend"
                CaseSensitive="False"
                Width="100px"
                ItemInsertLocation="Append" onselectedindexchanged="ComboBoxAllocated_SelectedIndexChanged"
                >
                <asp:ListItem Selected="True" Value="0">Unallocated</asp:ListItem>
                <asp:ListItem Value="1">Allocated</asp:ListItem>
                <asp:ListItem Value="*">All</asp:ListItem>
            </ajaxToolkit:ComboBox>
        </div>
        <div>
            <asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
        </div>
        </div>
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
                runat="server" ImageUrl="Images/detail.gif" PostBackUrl="~/purchordermaster.aspx"
                OnClientClick="return ImageButtonDetails_ClientClick()"
                />
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Edit</div>
        <div>
            <asp:ImageButton ID="ImageButtonEdit"
                runat="server" ImageUrl="Images/edit.gif" PostBackUrl="~/purchordermaster.aspx"
                OnClientClick="return ImageButtonEdit_ClientClick()"/>
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="~/images/delete.gif" PostBackUrl="~/purchordermaster.aspx"
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
            <cc1:jqGridAdv runat="server" ID="jqGridTable" colNames="['ID','Num','Type','Creditor','Bodycorp','Unit','Date','Allocated']"
             colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}, hidden:true },
                  { name: 'Num', index: 'Num', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Type', index: 'Type', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Creditor', index: 'Creditor', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Bodycorp', index: 'Bodycorp', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Unit', index: 'Unit', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Date', index: 'Date', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Allocated', index: 'Allocated', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
              ]"
             rowNum="25"
             rowList="[5, 10, 25, 50, 100]"
             sortname="ID"
             sortorder="asc"
             viewrecords="true"
             width="700"
             height="500"
             url="purchordermaster.aspx/DataGridDataBind"
             hasID="false"
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


