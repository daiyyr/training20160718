<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="menus.aspx.cs" Inherits="sapp_sms.menus" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Menus</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>

    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/menus.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">Menus</div>
            <div>
                <asp:TreeView ID="TreeViewMenus" runat="server" OnSelectedNodeChanged="TreeViewMenus_SelectedNodeChanged"
                    SelectedNodeStyle-ForeColor="DarkBlue" SelectedNodeStyle-VerticalPadding="0" ExpandDepth="1">
                   
                </asp:TreeView>
            </div>
            <div>
                <asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
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
        <div class="button-title">Edit</div>
        <div>
            <asp:ImageButton ID="ImageButtonEdit"
                runat="server" ImageUrl="Images/edit.gif" 
                onclick="ImageButtonEdit_Click" />
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Move Up</div>
        <div>
            <asp:ImageButton ID="ImageButtonMoveUp"
                runat="server" ImageUrl="Images/goup.gif" 
                onclick="ImageButtonMoveUp_Click"  />
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Move Down</div>
        <div>
            <asp:ImageButton ID="ImageButtonMoveDown"
                runat="server" ImageUrl="Images/godown.gif" 
                onclick="ImageButtonMoveDown_Click"  />
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="Images/delete.gif" OnClientClick="return confirm_delete();"
                onclick="ImageButtonDelete_Click" />
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
            <b>Sub Menus</b>
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridMenus" colNames="['ID', 'Name', 'Dir', 'ModuleId', 'Order']"
             colModel="[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: 'int', search: false, hidden:true},
             { name: 'Name', index: 'Name', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
             { name: 'Dir', index: 'Dir', width: 150, align: 'left', search: false},
             { name: 'ModuleId', index: 'ModuleId', width: 50, align: 'left', search: false},
             { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden:true}
             ]"
             rowNum="25"
             rowList="[5, 10, 25, 50, 100]"
             sortname="Order"
             sortorder="asc"
             viewrecords="true"
             width="700"
             height="500"
             url="menus.aspx/DataGridMenusDataBind"
             hasID="true"
             idName="menuid"
             />
            <asp:Button ID="ButtonSubAdd" runat="server" Text="Add Sub Menu" 
                onclick="ButtonSubAdd_Click" />
            <asp:Button ID="ButtonSubEdit" runat="server" Text="Edit Sub Menu" PostBackUrl="~/menus.aspx"
                OnClientClick="return ButtonSubEdit_ClientClick()"/>
            <asp:Button ID="ButtonSubDelete" runat="server" Text="Delete Sub Menu" PostBackUrl="~/menus.aspx"
                OnClientClick="return ButtonSubDelete_ClientClick()"/>
            <asp:Button ID="ButtonMoveUp" runat="server" Text="Sub Menu Up" 
                OnClientClick="return ButtonMoveUp_ClientClick()" />
            <asp:Button ID="ButtonMoveDown" runat="server" Text="Sub Menu Down" 
                OnClientClick="return ButtonMoveDown_ClientClick()" />
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
