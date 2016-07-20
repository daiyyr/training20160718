<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="accountdetails.aspx.cs" Inherits="sapp_sms.accountdetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Account Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/accountdetails.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="~/images/edit.gif"
                    OnClick="ImageButtonEdit_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                    OnClick="ImageButtonDelete_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" OnClientClick="history.back(); return false;"
                    ImageUrl="~/images/close.gif" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">General</a></li>
                <li><a href="#tabs-2">Comms</a></li>
            </ul>
            <div id="tabs-1">
                <table class="details">
                    <tr>
                        <td>
                            <b>Account:</b><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td colspan="3">
                            <b>
                                <asp:Label ID="LabelAccountID" runat="server" Text="ID" Visible="false"></asp:Label></b>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Code:</b><asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td>
                            <asp:Label ID="LabelCode" runat="server" Text="ID"></asp:Label>
                        </td>
                        <td>
                            <b>Chart Num:</b><asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td>
                            <asp:Label ID="LabelChart" runat="server" Text="ID"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Name:</b>
                        </td>
                        <td colspan="3">
                            <asp:Label ID="LabelName" runat="server" Text="ID"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Account Number:</b><asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td>
                            <asp:Label ID="LabelNumber" runat="server" Text="ID"></asp:Label>
                        </td>
                        <td>
                            <b>Bank:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelBank" runat="server" Text="ID"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Branch:</b><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td>
                            <asp:Label ID="LabelBranch" runat="server" Text="ID"></asp:Label>
                        </td>
                        <td>
                            <b>Swift Code:</b>
                        </td>
                        <td>
                            <asp:Label ID="LabelSwift" runat="server" Text="ID"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Description:</b><asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TextBoxDescription" runat="server" TextMode="MultiLine" Width="500px"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Image ID="Image10" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabs-2">
                <div>
                    <cc1:jqGridAdv runat="server" ID="jqGridComms" colNames="['ID', 'Type(*)', 'Details(*)', 'Primary', 'Order']"
                        colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                     { name: 'Type', index: 'Type', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'accountdetails.aspx/DataGridCommsTypeSelect'}, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                     { name: 'Details', index: 'Details', width: 200, editable:true,editoptions: { 'maxlength': 100 } ,editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: false},
                     { name: 'Primary', index: 'Primary', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'Yes:No'}, align: 'left', search: false},
                     { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden: true}
                     ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Order" sortorder="asc" viewrecords="true"
                        width="700" height="500" url="accountdetails.aspx/DataGridCommsDataBind" hasID="true"
                        idName="accountid" inlineNav="true" editurl="accountdetails.aspx/DataGridCommsSave"
                        contentPlaceHolder="ContentPlaceHolder1" />
                </div>
                <div align="left">
                    <asp:Button ID="ButtonDeleteComm" runat="server" Text="Delete" OnClientClick="return ButtonDeleteComm_ClientClick()" />
                </div>
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
