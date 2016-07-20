<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptycntrmasterdetails.aspx.cs" Inherits="sapp_sms.pptycntrmasterdetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Property Contractor Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/pptycntrmasterdetails.js" type="text/javascript"></script>
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
            <td><b>Element ID:</b><asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID"  Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            
            <td><b>Service :</b><asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td><asp:Label ID="LabelService" runat="server" ></asp:Label>
                &nbsp;</td>
            <td><b>Property :</b></td>
            <td><asp:Label ID="LabelProperty" runat="server" ></asp:Label>
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            
            <td><b>Creditor :</b><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td><asp:Label ID="LabelCreditor" runat="server" ></asp:Label>
                &nbsp;</td>
            <td><b>Expiry:</b></td>
            <td><asp:Label ID="LabelExpiry" runat="server" ></asp:Label>
                &nbsp;</td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Inactive :</b><asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td><asp:Label ID="LabelInactive" runat="server" ></asp:Label>
                &nbsp;</td>
            <td><b></b></td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            <td><b>Notes:</b><asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="3"><asp:Label ID="LabelNotes" runat="server" ></asp:Label>
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
    </table>
    </div>
    <div id="content_right">
        <div class="button">
        <div class="button-title">Creditor</div>
        <div>
            <asp:ImageButton ID="ImageButtonCreditor" runat="server" 
                ImageUrl="~/images/contracts_v2.gif" onclick="ImageButtonCreditor_Click" />
        </div>
    </div>
    </div>
</asp:Content>

