<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="chartmasterdetails.aspx.cs" Inherits="sapp_sms.chartmasterdetails" %>
<%@ Register TagPrefix="uc" TagName="jqGrid" Src="~/jqGrid.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Chart Master Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/chartmasterdetails.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
    <div class="button">
        <div class="button-title">Edit</div>
        <div>
            <asp:ImageButton ID="ImageButtonEdit"
                runat="server" ImageUrl="~/images/edit.gif" 
                onclick="ImageButtonEdit_Click" />
        </div>
    </div>
    <div class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="~/images/delete.gif"
                OnClientClick="return confirm_delete();" 
                onclick="ImageButtonDelete_Click" />
        </div>
    </div>
    <div class="button">
        <div class="button-title">Close</div>
        <div>
            <asp:ImageButton ID="ImageButtonClose" runat="server"  OnClientClick="history.back(); return false;"
                ImageUrl="~/images/close.gif" onclick="ImageButtonClose_Click" />
        </div>
    </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td><b>Chart Master:</b><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td colspan="3">
                    <b><asp:Literal ID="LiteralClientID" runat="server" Text="ID"  Visible="false"></asp:Literal></b>
                </td>
            </tr>
            <tr>
                <td colspan="4"><asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            </tr>

            <tr>
                <td><b>Code:</b></td>
                <td>
                    <asp:Label ID="LabelCode" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
                <td><b>Type:</b></td>
                <td>
                    <asp:Label ID="LabelType" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
            </tr>
             <tr>
                <td colspan="4"><asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            </tr>
            <tr>
                <td><b>Name:</b><asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td colspan="3">
                    <asp:Label ID="LabelName" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
            </tr>
             <tr>
                <td colspan="4"><asp:Image ID="Image13" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            </tr>
            <tr>
                <td><b>Recharge:</b><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td>
                    <asp:Label ID="LabelRecharge" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
                <td><b>Account:</b><asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td>
                    <asp:Label ID="LabelAccount" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    &nbsp;</td>
                <td><b>Parent</b></td>
                <td>
                    <asp:Label ID="LabelParent" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
            </tr>
             <tr>
                <td colspan="4"><asp:Image ID="Image14" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            </tr>

            <tr>
                <td><b>Notax:</b><asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                    <asp:Label ID="LabelNotax" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
                <td><b>Levy Base:</b><asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td>
                    <asp:CheckBox ID="CheckBox2" runat="server" />
                    <asp:Label ID="LabelLevyBase" runat="server" CssClass="sapplabel"></asp:Label>
                    <asp:Label ID="LabelBank" runat="server" CssClass="sapplabel" Visible="false"></asp:Label>
                </td>
            </tr>
             <tr>
                <td colspan="4"><asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            </tr>

            <tr>
                <td><b>Trust Account:</b><asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td>
                    <asp:CheckBox ID="CheckBox3" runat="server" />
                    <asp:Label ID="LabelTrust" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
                <td><b>Inactive:</b><asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
                <td>
                    <asp:CheckBox ID="CheckBox4" runat="server" />
                    <asp:Label ID="LabelInactive" runat="server" CssClass="sapplabel"></asp:Label>
                </td>
            </tr>

            <tr>
                <td><b>Bank Account :</b></td>
                <td>
                <asp:CheckBox ID="CheckBoxBankAccount" runat="server" Enabled="False" />
                </td>
                <td>&nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
             <tr>
                <td colspan="4"><asp:Image ID="Image10" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            </tr>
            
        </table>  
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title"></div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
