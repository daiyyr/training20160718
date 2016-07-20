<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="privileges.aspx.cs" Inherits="sapp_sms.privileges" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Privileges</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/privileges.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Privileges</div>
            <div>
                <ajaxToolkit:ComboBox ID="ComboBoxPrivileges" runat="server" AutoPostBack="True"
                    DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" CaseSensitive="False"
                    ItemInsertLocation="Append" Width="100px" OnSelectedIndexChanged="ComboBoxPrivileges_SelectedIndexChanged">
                </ajaxToolkit:ComboBox>
            </div>
            <div>
                <asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Add</div>
            <div>
                <asp:ImageButton ID="ImageButtonAdd" runat="server" ImageUrl="Images/new.gif" OnClick="ImageButtonAdd_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="Images/edit.gif" OnClick="ImageButtonEdit_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="Images/delete.gif"
                    OnClientClick="return confirm_delete();" OnClick="ImageButtonDelete_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false" OnClientClick="history.back(); return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Form Authorisation</a></li>
                <li><a href="#tabs-2">Menu Authorisation</a></li>
            </ul>
            <div id="tabs-1">
                <table class="details">
                    <tr>
                        <td>
                            <b>Unauthorised Forms</b>
                        </td>
                        <td>
                            <b>Authorised Forms</b>
                        </td>
                    </tr>
                    <tr style="border-color: #000 #000 #000 #000;">
                        <td style="border-color: #000 #000 #000 #000;">
                            <img src="Images/dot.gif" height="4px" />
                            <cc1:jqGridAdv runat="server" ID="jqGridForms" colNames="['ID', 'Name', 'FileName']"
                                colModel="[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                            { name: 'Name', index: 'Name', width: 150, align: 'left', search: false },
                            { name: 'FileName', index: 'FileName', width: 150, align: 'left', search: false }
                            ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" multiselect="true"
                                viewrecords="true" width="380" height="500" url="privileges.aspx/DataGridFormsDataBind"
                                hasID="true" idName="privilegeid" onSelectRow="" />
                        </td>
                        <td>
                            <img src="Images/dot.gif" height="4px" />
                            <cc1:jqGridAdv runat="server" ID="jqGridIncludedForms" colNames="['ID', 'Name', 'FileName']"
                                colModel="[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true },
                            { name: 'Name', index: 'Name', width: 150, align: 'left', search: false },
                            { name: 'FileName', index: 'FileName', width: 150, align: 'left', search: false }
                            ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                                width="380" height="500" url="privileges.aspx/DataGridIncludedFormsDataBind"
                                hasID="true" idName="privilegeid" multiselect="true" onSelectRow="" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:ImageButton ID="ImageButtonMoveLeft" runat="server" ImageUrl="Images/goback.gif"
                                PostBackUrl="~/privileges.aspx" OnClientClick="return ImageButtonMoveLeft_ClientClick()" />
                            <asp:ImageButton ID="ImageButtonMoveRight" runat="server" ImageUrl="Images/goright.gif"
                                PostBackUrl="~/privileges.aspx" OnClientClick="return ImageButtonMoveRight_ClientClick()" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabs-2">
                <table class="details">
                    <tr>
                        <td>
                            <b>Unauthorised Menus</b>
                        </td>
                        <td>
                            <b>Authorised Menus</b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <img src="Images/dot.gif" height="4px" />
                            <cc1:jqGridAdv runat="server" ID="jqGridMenus" colNames="['ID', 'Name', 'Parent']"
                                colModel="[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} },
                            { name: 'Name', index: 'Name', width: 150, align: 'left', search: false },
                            { name: 'Parent', index: 'Parent', width: 150, align: 'left', search: false }
                            ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                                width="380" height="500" url="privileges.aspx/DataGridMenusDataBind" hasID="true"
                                idName="privilegeid" multiselect="true" onSelectRow="" />
                        </td>
                        <td>
                            <img src="Images/dot.gif" height="4px" />
                            <cc1:jqGridAdv runat="server" ID="jqGridIncludedMenus" colNames="['ID', 'Name', 'Parent']"
                                colModel="[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} },
                            { name: 'Name', index: 'Name', width: 150, align: 'left', search: false },
                            { name: 'Parent', index: 'Parent', width: 150, align: 'left', search: false }
                            ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                                width="380" height="500" url="privileges.aspx/DataGridIncludedMenusDataBind"
                                hasID="true" idName="privilegeid" multiselect="true" onSelectRow="" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:ImageButton ID="ImageButtonMenuLeft" runat="server" ImageUrl="Images/goback.gif"
                                PostBackUrl="~/privileges.aspx" OnClientClick="return ImageButtonMenuLeft_ClientClick()" />
                            <asp:ImageButton ID="ImageButtonMenuRight" runat="server" ImageUrl="Images/goright.gif"
                                PostBackUrl="~/privileges.aspx" OnClientClick="return ImageButtonMenuRight_ClientClick()" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
            </div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
