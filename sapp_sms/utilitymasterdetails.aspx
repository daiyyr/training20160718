<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="utilitymasterdetails.aspx.cs" Inherits="sapp_sms.utilitymasterdetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Utility Master Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/utilitymasterdetails.js" type="text/javascript"></script>
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
            <asp:ImageButton ID="ImageButtonClose" runat="server"  OnClientClick="history.back();; return false;"
                ImageUrl="~/images/close.gif" onclick="ImageButtonClose_Click" />
        </div>
    </div>
    </div>
    <div id="content_middle">
         <table class="details">
        <tr>
            <td><b>Element ID:</b><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            <td><b>Unit:</b><asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td>
                <asp:Label ID="LabelUnit" runat="server"></asp:Label>
                </td>
            <td><b>Type:</b></td>
            <td>
                <asp:Label ID="LabelType" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Reading:</b><asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td>
                <asp:Label ID="LabelReading" runat="server"></asp:Label>
                   </td>
            <td><b>Date:</b></td>
            <td>
                <asp:Label ID="LabelDate" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Batch:</b><asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td>
                <asp:Label ID="LabelBatch" runat="server"></asp:Label>
                   </td>
            <td><b>Unit Price:</b></td>
            <td>
                <asp:Label ID="LabelUnitPrice" runat="server"></asp:Label>
                   </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        

        <tr>
            <td><b>Notes:</b><asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="3">
                <asp:Label ID="LabelNotes" runat="server"></asp:Label>
            </td>
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
